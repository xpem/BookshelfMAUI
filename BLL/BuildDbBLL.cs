using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BLL
{
    public class BuildDbBLL : IBuildDbBLL
    {
        private readonly BookshelfDbContext bookshelfDBContext;

        public BuildDbBLL(BookshelfDbContext bookshelfDBContext)
        {
            this.bookshelfDBContext = bookshelfDBContext;
        }

        public async Task Init()
        {

            await bookshelfDBContext.Database.EnsureCreatedAsync();

            VersionDbTables? actualVesionDbTables = await bookshelfDBContext.VersionDbTables.FirstOrDefaultAsync();

            VersionDbTables newVersionDbTables = new() { Id = 0, VERSION = 1 };

            if (actualVesionDbTables != null)
            {
                if (actualVesionDbTables.VERSION != newVersionDbTables.VERSION)
                {
                    await bookshelfDBContext.Database.EnsureDeletedAsync();

                    await bookshelfDBContext.Database.EnsureCreatedAsync();
                }
            }
            else
            {
                bookshelfDBContext.VersionDbTables.Add(newVersionDbTables);
                await bookshelfDBContext.SaveChangesAsync();
            }

        }

        public async Task CleanLocalDatabase()
        {
            //uma alternativa é criar um campo que define se o usuario está logado localmente ou n. vinculado ao uid externo dele(seria necessário a api passá-lo)
            //assim na troca de usuário ou signout, as informações sincronizadas nao seriam perdidas, mas para isso é necessário verificar a necessidade dessa função
            //no simples, só esvazia o banco msm p ser recriado no signin novo.

            await bookshelfDBContext.Database.EnsureDeletedAsync();

        }
    }
}
