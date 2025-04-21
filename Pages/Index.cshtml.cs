using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using k7.Service;

namespace k7.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string A { get; set; }
        [BindProperty] public string B { get; set; }

        // 只留一個共用的 Result
        public string Result { get; private set; }

        public void OnGet() { }

        // 按「加法」時走這裡
        public void OnPostAdd()
        {
            Result = BigNumberService.Add(A, B);
        }

        // 按「減法」時走這裡
        public void OnPostSubtract()
        {
            Result = BigNumberService.Subtract(A, B);
        }

        // 按「乘法」時走這裡
        public void OnPostMultiply()
        {
            Result = BigNumberService.Multiply(A, B);
        }
    }
}
