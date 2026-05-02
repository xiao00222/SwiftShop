using SwiftShop.Models;
using SwiftShop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftShop.DataAccess.Repository
{
    public interface ICompanyRepository: IRepository<Company>
    {
        void Update(Company company);
    }
}
