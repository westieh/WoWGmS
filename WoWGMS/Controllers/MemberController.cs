using Microsoft.AspNetCore.Mvc;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;

namespace WoWGMS.Controllers
{
    public class MemberController : Controller
    {
        private readonly IDBService<Member>? _memberService;

        public MemberController(IDBService<Member> memberService)
        {
            _memberService = memberService;
        }
        //READ
        public async Task<IActionResult> GetAllObjects()
        {
            var members = await _memberService.GetAllObjectsAsync();
            return View(members);
        }
        //CREATE
        public async Task<IActionResult> CreateMember(Member member)
        {
            await _memberService.AddObjectAsync(member);
            return RedirectToAction("Index");
        }
        //DELETE - GET
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _memberService.GetObjectByIdAsync(id);
            if (member == null) return View("Error");
            return View(member);
        }
        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {

            var member = await _memberService.GetObjectByIdAsync(id);
            if (member == null) return View("Error");

            await _memberService.DeleteObjectAsync(member);
            return RedirectToAction("Index");
        }
    }
}
