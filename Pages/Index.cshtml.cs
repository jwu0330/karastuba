using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using karastsuba.Service;

namespace karatsuba.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string A { get; set; }
        [BindProperty] public string B { get; set; }

        // �u�d�@�Ӧ@�Ϊ� Result
        public string Result { get; private set; }

        public void OnGet() { }

        // ���u�[�k�v�ɨ��o��
        public void OnPostAdd()
        {
            Result = BigNumberService.Add(A, B);
        }

        // ���u��k�v�ɨ��o��
        public void OnPostSubtract()
        {
            Result = BigNumberService.Subtract(A, B);
        }

        // ���u���k�v�ɨ��o��
        public void OnPostMultiply()
        {
            Result = BigNumberService.Multiply(A, B);
        }
    }
}
