using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymAPI.Controllers
{
    public interface IReactAdminController<T>
    {
        Task<ActionResult<IEnumerable<T>>> Get(string filter = "", string range = "", string sort = "");
        Task<ActionResult<T>> Get(int ID);
        Task<IActionResult> Put(int ID, T entity);
        Task<ActionResult<T>> Post(T entity);
        Task<ActionResult<T>> Delete(int ID);
    }
}
