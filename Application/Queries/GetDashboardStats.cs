using Application.Common.Dtos;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries
{
    public class GetDashboardStats
    {
        public record GetDashboardStatsQuery()
            : IRequest<Result<GetDashboardStatsResponse>>;

        public class GetDashboardStatsHandler(
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IWalletTransactionRepository walletTransactionRepository)
            : IRequestHandler<GetDashboardStatsQuery,
                Result<GetDashboardStatsResponse>>
        {
            public async Task<Result<GetDashboardStatsResponse>> Handle(
                GetDashboardStatsQuery request,
                CancellationToken cancellationToken)
            {
                var customers = await customerRepository.GetCustomersAsync();
                var orders = await orderRepository.GetAllAsync();
                var products = await productRepository.GetAllAsync();
                var transactions = await walletTransactionRepository.GetAllAsync();

                return Result<GetDashboardStatsResponse>.Success(
                    "Success",
                    new GetDashboardStatsResponse(
                        customers.Count(c => !c.IsDeleted),
                        orders.Count(o => !o.IsDeleted),
                        orders.Count(o => o.Status == Domain.Enums.OrderStatus.Pending),
                        products.Count(p => !p.IsDeleted),
                        products.Count(p => p.IsLowStock && !p.IsDeleted),
                        transactions
                            .Where(t => t.Type == Domain.Enums.TransactionType.Credit
                                && t.Status == WalletTransactionStatus.Success)
                            .Sum(t => t.Amount)));
            }
        }

        public record GetDashboardStatsResponse(
            int TotalCustomers,
            int TotalOrders,
            int PendingOrders,
            int TotalProducts,
            int LowStockProducts,
            decimal TotalRevenue);
    }
}