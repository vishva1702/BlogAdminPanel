//using BlogAdminPanel.DTO;
//using Microsoft.AspNetCore.Mvc;
//using BlogAdminPanel.Models;

//namespace BlogAdminPanel.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class CommentController : Controller
//    {
//        private readonly CommentController _moderationService;

//        public CommentController(CommentController moderationService)
//        {
//            _moderationService=moderationService;
//        }

//        [HttpGet("/comments")]
//        public IActionResult Index()
//        {
//            return View();
//            //try
//            //{
//            //    // Fetch data to display in the view
//            //    var comments = _moderationService.GetAllComments();
//            //    return View(comments); // Pass data to the view
//            //}
//            //catch (Exception ex)
//            //{
//            //    // Handle any issues and return an error page if needed
//            //    return BadRequest(new { error = ex.Message });
//            //}
//        }

//        private string? GetAllComments()
//        {
//            throw new NotImplementedException();
//        }

//        [HttpPost("{commentId}/approve")]
//        public IActionResult ApproveComment(int commentId, [FromBody] string moderator)
//        {
//            try
//            {
//                _moderationService.ApproveComment(commentId, moderator);
//                return Ok("Comment approved successfully.");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPost("{commentId}/reject")]
//        public IActionResult RejectComment(int commentId, string moderator, [FromBody] RejectRequest request)
//        {
//            try
//            {
//                _moderationService.RejectComment(commentId, request.Moderator, request.Reason);
//                return Ok(new { message = "Comment rejected successfully." });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { error = ex.Message });
//            }
//        }

//        private void RejectComment(int commentId, string moderator, string reason)
//        {
//            throw new NotImplementedException();
//        }

//        [HttpPost("{commentId}/flag")]
//        public IActionResult FlagComment(int commentId, string moderator, [FromBody] FlagRequest request)
//        {
//           try
//           {
//                _moderationService.FlagComment(commentId, request.Moderator, request.Reason);
//                 return Ok(new { message = "Comment flagged successfully." });
//           }
//            catch (Exception ex)
//            {
//                return BadRequest(new { error = ex.Message });
//            }
//        }

//        private void FlagComment(int commentId, string moderator, string reason)
//        {
//            throw new NotImplementedException();
//        }

//        [HttpGet("{commentId}/audit")]
//        public IActionResult GetAuditTrail(int commentId)
//        {
//            try
//            {
//                var auditTrail = _moderationService.GetAuditTrail(commentId);
//                return Ok(auditTrail);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }
//    }
//}
