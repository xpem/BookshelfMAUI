using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDAL.Interfaces
{
    public interface IBookHistoricApiDAL
    {
        Task<ApiResponse> GetBooksHistoricByLastCreatedAt(DateTime lastUpdate);
    }
}
