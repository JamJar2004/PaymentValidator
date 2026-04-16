using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentValidator.Interfaces
{
    public interface ILogger<TMessage>
    {
        Task LogAsync(TMessage message);
    }
}
