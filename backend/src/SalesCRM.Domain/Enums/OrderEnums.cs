namespace SalesCRM.Domain.Enums;

public enum OrderStatus
{
    Draft,
    Confirmed,
    Paid,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    VNPAY,
    MoMo,
    BankTransfer
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}

public enum TransactionType
{
    Import,      // Nhập kho
    Export,      // Xuất kho
    Replenish,   // Bồi hàng lên kệ
    TransferOut, // Chuyển đi chi nhánh khác
    TransferIn,  // Nhận từ chi nhánh khác
    WriteOff     // Hủy hàng hỏng/hết hạn
}

public enum TransferStatus
{
    Pending,
    Shipped,
    Received,
    Cancelled
}
