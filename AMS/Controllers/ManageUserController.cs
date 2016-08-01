using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AMS.Constant;
using AMS.Enum;
using AMS.Filter;
using AMS.Models;
using AMS.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
namespace AMS.Controllers
{
    public class ManageUserController : Controller
    {
        private UserServices _userServices = new UserServices();
        private BlockServices _blockServices = new BlockServices();
        private HouseServices _houseServices = new HouseServices();
        private UserService userService = new UserService();
        private PostService _postService = new PostService();

        [HttpGet]
        [ManagerAuthorize]
        [Route("Management/ManageUser/ViewResidentList")]
        public ActionResult ViewResidentList()
        {
            return View("ManageResident");
        }

        [HttpGet]
        [ManagerAuthorize]
        [Route("Management/ManageUser/ViewSupporterList")]
        public ActionResult ViewSupporterList()
        {
            return View("ManageSupporter");
        }

        [HttpGet]
        [AdminAuthorize]
        [Route("Management/ManageUser/ViewManagerList")]
        public ActionResult ViewManagerList()
        {
            return View("~/Views/Config/ManageManager.cshtml");
        }

        [HttpGet]
        [Route("Management/ManageUser/GetResidentList")]
        public ActionResult GetUserList()
        {
            List<User> listUsers = _userServices.GetAllResident();
            List<UserInfoViewModel> listModelUsers = new List<UserInfoViewModel>();
            foreach (var user in listUsers)
            {
                UserInfoViewModel userModel = new UserInfoViewModel();
                userModel.Id = user.Id;
                userModel.DT_RowId = new StringBuilder("resident_").Append(user.Id).ToString();
                userModel.Name = user.Fullname;
                userModel.Idenity = user.IDNumber;
                userModel.Status = user.Status.Value;
                if (userModel.Status != SLIM_CONFIG.USER_STATUS_DISABLE)
                {
                    userModel.Block = user.House.Block.BlockName;
                    userModel.Floor = user.House.Floor;
                    userModel.HouseName = user.House.HouseName;
                }
                else
                {
                    userModel.HouseName = "-1";

                }
                userModel.CreateDate = user.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                userModel.CreateDateLong = user.CreateDate.Value.Ticks;
                userModel.RoldId = user.RoleId.Value;
                userModel.RolName = user.Role.RoleName;

                userModel.IsDeletable = 1;// can delete
                if (userModel.Status != SLIM_CONFIG.USER_STATUS_DISABLE && userModel.RoldId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)
                {
                    if (user.House.Users.Count != 1)
                    {
                        userModel.IsDeletable = 2;// can not delete
                    }
                }
                listModelUsers.Add(userModel);
            }
            return Json(listModelUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageUser/GetEmployeeList")]
        public ActionResult GetEmployeeList(int roleId)
        {
            List<User> listUsers = new List<User>();
            if (roleId == SLIM_CONFIG.USER_ROLE_SUPPORTER)
            {
                listUsers = _userServices.GetAllSupporter();
            }
            else if (roleId == SLIM_CONFIG.USER_ROLE_MANAGER)
            {

            }
            else
            {
                return Json(listUsers, JsonRequestBehavior.AllowGet);
            }

            List<UserInfoViewModel> listModelUsers = new List<UserInfoViewModel>();
            foreach (var user in listUsers)
            {
                UserInfoViewModel userModel = new UserInfoViewModel();
                userModel.Id = user.Id;
                userModel.DT_RowId = new StringBuilder("employee_").Append(user.Id).ToString();
                userModel.Name = user.Fullname;
                userModel.Idenity = user.IDNumber;
                userModel.CreateDate = user.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                userModel.CreateDateLong = user.CreateDate.Value.Ticks;
                userModel.RoldId = user.RoleId.Value;
                userModel.RolName = user.Role.RoleName;
                userModel.Status = user.Status.Value;
                listModelUsers.Add(userModel);
            }
            return Json(listModelUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageUser/GetManagerList")]
        public ActionResult GetManagerList()
        {
            List<User> listUsers = new List<User>();

            listUsers = _userServices.GetAllManager();

            List<UserInfoViewModel> listModelUsers = new List<UserInfoViewModel>();
            foreach (var user in listUsers)
            {
                UserInfoViewModel userModel = new UserInfoViewModel();
                userModel.Id = user.Id;
                userModel.DT_RowId = new StringBuilder("employee_").Append(user.Id).ToString();
                userModel.Name = user.Fullname;
                userModel.Idenity = user.IDNumber;
                userModel.CreateDate = user.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                userModel.CreateDateLong = user.CreateDate.Value.Ticks;
                userModel.RoldId = user.RoleId.Value;
                userModel.RolName = user.Role.RoleName;
                userModel.Status = user.Status.Value;
                listModelUsers.Add(userModel);
            }
            return Json(listModelUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageUser/GetHouseList")]
        public ActionResult GetHouseList()
        {
            MessageViewModels response = new MessageViewModels();

            List<Block> blocks = _blockServices.GetAllBlocks();
            List<HouseCategoryModel> blockList = new List<HouseCategoryModel>();
            List<HouseCategoryModel> floorList = new List<HouseCategoryModel>();
            List<HouseCategoryModel> roomList = new List<HouseCategoryModel>();

            HouseCategoryModel item = null;
            foreach (var block in blocks)
            {
                item = new HouseCategoryModel();
                item.Id = block.Id;
                item.Name = block.BlockName;
                blockList.Add(item);
            }
            if (blocks.Count != 0)
            {
                List<House> floors = _houseServices.GetAllFloorInBlock(blocks[0].Id);
                foreach (var floor in floors)
                {
                    item = new HouseCategoryModel();
                    item.Id = floor.Id;
                    item.Name = floor.Floor;
                    floorList.Add(item);
                }

                List<House> rooms = _houseServices.GetAllRoomsInFloor(blocks[0].Id, floors[0].Floor);
                foreach (var room in rooms)
                {
                    item = new HouseCategoryModel();
                    item.Id = room.Id;
                    item.Name = room.HouseName;
                    roomList.Add(item);
                }
            }
            response.Data = new
            {
                blocks = blockList,
                floors = floorList,
                rooms = roomList
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageUser/GetResidentInfor")]
        public ActionResult GetUserList(int residentId)
        {
            MessageViewModels response = new MessageViewModels();
            User resident = _userServices.FindById(residentId);
            if (null != resident)
            {
                UserInfoViewModel userModel = new UserInfoViewModel();
                userModel.Id = resident.Id;
                userModel.Name = resident.Fullname;
                userModel.Idenity = resident.IDNumber;

                userModel.Gender = resident.Gender.Value;
                userModel.Dob = resident.DateOfBirth.Value.ToString(AmsConstants.DateFormat);
                userModel.CreateDate = resident.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                userModel.IdCreateDate = resident.IDCreatedDate == null ? "" : resident.CreateDate.Value.ToString(AmsConstants.DateFormat);
                userModel.RoldId = resident.RoleId.Value;
                userModel.RolName = resident.Role.RoleName;
                userModel.Status = resident.Status.Value;
                userModel.RelationLevel = resident.FamilyLevel.Value; ;
                userModel.CellNumb = resident.SendPasswordTo; ;
                userModel.UserAccountName = resident.Username;

                if (userModel.Status != SLIM_CONFIG.USER_STATUS_DISABLE)
                {
                    userModel.Block = resident.House.Block.BlockName;
                    userModel.BlockId = resident.House.Block.Id;
                    userModel.Floor = resident.House.Floor;
                    userModel.HouseId = resident.House.Id;
                }
                else
                {
                    userModel.Block = "";
                    userModel.BlockId = 0;
                    userModel.Floor = "";
                    userModel.HouseId = 0;
                }

                List<Block> blocks = _blockServices.GetAllBlocks();
                List<HouseLocationModel> blockList = new List<HouseLocationModel>();
                List<HouseLocationModel> floorList = new List<HouseLocationModel>();
                List<HouseLocationModel> roomList = new List<HouseLocationModel>();

                HouseLocationModel item = null;
                foreach (var block in blocks)
                {
                    item = new HouseLocationModel();
                    item.Id = block.Id.ToString();
                    item.Name = block.BlockName;
                    blockList.Add(item);
                }
                if (blocks != null && blocks.Count != 0)
                {
                    List<House> floors = null;
                    List<House> rooms = null;

                    if (userModel.Status != SLIM_CONFIG.USER_STATUS_DISABLE)
                    {
                        floors = _houseServices.GetAllFloorInBlock(resident.House.Block.Id);
                        rooms = _houseServices.GetAllRoomsInFloor(resident.House.Block.Id, resident.House.Floor);
                    }
                    else
                    {
                        floors = _houseServices.GetAllFloorInBlock(Int32.Parse(blockList[0].Id));
                        rooms = _houseServices.GetAllRoomsInFloor(Int32.Parse(blockList[0].Id), floors[0].Floor);
                    }

                    foreach (var floor in floors)
                    {
                        item = new HouseLocationModel();
                        item.Id = floor.Floor;
                        item.Name = floor.Floor;
                        floorList.Add(item);
                    }

                    foreach (var room in rooms)
                    {
                        item = new HouseLocationModel();
                        item.Id = room.Id.ToString();
                        item.Name = room.HouseName;
                        roomList.Add(item);
                    }
                }
                object data = new
                {
                    data = userModel,
                    blocks = blockList,
                    floors = floorList,
                    rooms = roomList
                };

                response.Data = data;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ManageUser/GetEmployeeInfor")]
        public ActionResult GetSupporterInfor(int supporterId)
        {
            MessageViewModels response = new MessageViewModels();
            User supporter = _userServices.FindById(supporterId);
            if (null != supporter)
            {
                UserInfoViewModel userModel = new UserInfoViewModel();
                userModel.Id = supporter.Id;
                userModel.Name = supporter.Fullname;
                userModel.Idenity = supporter.IDNumber;
                userModel.Gender = supporter.Gender.Value;
                userModel.Dob = supporter.DateOfBirth.Value.ToString(AmsConstants.DateFormat);
                userModel.CreateDate = supporter.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                userModel.IdCreateDate = supporter.IDCreatedDate == null ? "" : supporter.IDCreatedDate.Value.ToString(AmsConstants.DateFormat);
                userModel.UserAccountName = supporter.Username;
                userModel.RoldId = supporter.RoleId.Value;
                userModel.RolName = supporter.Role.RoleName;
                userModel.CellNumb = supporter.SendPasswordTo;
                userModel.Status = supporter.Status.Value;

                response.Data = userModel;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/ManageUser/AddNewResident")]
        public ActionResult AddNewResident(UserInfoViewModel user)
        {
            MessageViewModels response = new MessageViewModels();
            if (null != user.HouseId)
            {
                try
                {
                    House house = _houseServices.FindById(user.HouseId);
                    if (null != house)
                    {
                        User u = new User();
                        if (house.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE && house.OwnerID == null && user.IsHouseOwner == SLIM_CONFIG.USER_ROLE_RESIDENT)
                        {
                            response.StatusCode = 2;
                            response.Msg = "Nhà này vẫn chưa có chủ hộ !";
                            return Json(response);
                        }

                        u.Fullname = user.Name;
                        u.HouseId = house.Id;
                        u.CreateDate = DateTime.Now;
                        u.LastModified = DateTime.Now;
                        u.IDNumber = user.Idenity;
                        u.Status = SLIM_CONFIG.USER_STATUS_ENABLE;
                        u.Gender = user.Gender;
                        u.DateOfBirth = DateTime.ParseExact(user.Dob, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                        if (user.IdCreateDate != null)
                        {
                            u.IDCreatedDate = DateTime.ParseExact(user.IdCreateDate, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                        }
                        u.Username = user.UserAccountName;
                        u.Creator = Int32.Parse(User.Identity.GetUserId());
                        u.RoleId = user.IsHouseOwner;
                        u.FamilyLevel = user.RelationLevel;
                        u.Password = CommonUtil.GetUniqueKey(8);
                        u.SendPasswordTo = user.CellNumb;
                        _userServices.Add(u);
                        if (user.IsHouseOwner == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)
                        {
                            house.OwnerID = u.Id;
                            _houseServices.Update(house);

                            foreach (var userInHouse in house.Users.Where(usr => usr.Id != u.Id && u.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)
                                )
                            {
                                User usr = _userServices.FindById(userInHouse.Id);
                                usr.RoleId = SLIM_CONFIG.USER_ROLE_RESIDENT;
                                usr.LastModified = DateTime.Now;
                                _userServices.Update(usr);
                            }
                        }

                        StringBuilder message = new StringBuilder();
                        message.Append("Chung cu AMS. Tai khoan duoc tao thanh cong! Ten đang nhap: ")
                            .Append(u.Username)
                            .Append(". Mat khau: ")
                            .Append(u.Password);
                        CommonUtil.SentSms(u.SendPasswordTo, message.ToString());

                        //                    var accountSid = "AC10ae7ed64035004a9f1ed772747b94dc"; // Your Account SID from www.twilio.com/console
                        //                    var authToken = "c867c6dadb271752b1fa0bb988f1c284";  // Your Auth Token from www.twilio.com/console
                        //
                        //                    var twilio = new TwilioRestClient(accountSid, authToken);
                        //                    var message = twilio.SendMessage(
                        //                        "+12057198424", // From (Replace with your Twilio number)
                        //                        "+84934876200", // To (Replace with your phone number)
                        //                        "One time password: chào bạn"
                        //                        );
                        //
                        //                    Console.WriteLine(message.Sid);
                        //                    Console.Write("Press any key to continue.");
                        //                    Console.ReadKey();
                    }
                    else
                    {
                        response.StatusCode = -1;
                    }
                }
                catch (Exception)
                {
                    response.StatusCode = -1;
                    return Json(response);
                }
            }
            else
            {
                response.StatusCode = -1;
            }

            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageUser/AddEmployee")]
        public ActionResult AddNewSupporter(UserInfoViewModel user)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                User u = parseAddDataUser(user, SLIM_CONFIG.USER_ROLE_SUPPORTER);
                u.FamilyLevel = SLIM_CONFIG.FAMILY_LEVEL_SUPPORTER;
                _userServices.Add(u);
                StringBuilder message = new StringBuilder();
                message.Append("Chung cu AMS. Tai khoan duoc tao thanh cong! Ten đang nhap: ")
                    .Append(u.Username)
                    .Append(". Mat khau: ")
                    .Append(u.Password);
                CommonUtil.SentSms(u.SendPasswordTo, message.ToString());
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageUser/AddManager")]
        public ActionResult AddNewManager(UserInfoViewModel user)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                User u = parseAddDataUser(user, SLIM_CONFIG.USER_ROLE_MANAGER);
                u.FamilyLevel = SLIM_CONFIG.FAMILY_LEVEL_ADMIN;
                _userServices.Add(u);
                StringBuilder message = new StringBuilder();
                message.Append("Chung cu AMS. Tai khoan duoc tao thanh cong! Ten đang nhap: ")
                    .Append(u.Username)
                    .Append(". Mat khau: ")
                    .Append(u.Password);
                CommonUtil.SentSms(u.SendPasswordTo, message.ToString());
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }


        [HttpPost]
        [Route("Management/ManageUser/UpdateResident")]
        public ActionResult UpdateResident(UserInfoViewModel user)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                House house = _houseServices.FindById(user.HouseId);
                if (null != house)
                {
                    User u = _userServices.FindById(user.Id);
                    if (null != u)
                    {
                        if (house.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE && house.OwnerID == null && user.IsHouseOwner == SLIM_CONFIG.USER_ROLE_RESIDENT)
                        {
                            response.StatusCode = 2;
                            response.Msg = "Nhà này vẫn chưa có chủ hộ !";
                            return Json(response);
                        }
                        u.Fullname = user.Name;
                        u.HouseId = house.Id;
                        u.LastModified = DateTime.Now;
                        u.IDNumber = user.Idenity;
                        u.Gender = user.Gender;
                        u.DateOfBirth = DateTime.ParseExact(user.Dob, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
                        u.LastModified = DateTime.Now;
                        u.FamilyLevel = user.RelationLevel;
                        u.SendPasswordTo = user.CellNumb;
                        if (user.IdCreateDate != null)
                        {
                            u.IDCreatedDate = DateTime.ParseExact(user.IdCreateDate, AmsConstants.DateFormat,
                                CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            u.IDCreatedDate = null;
                        }
                        u.Status = SLIM_CONFIG.USER_STATUS_ENABLE;
                        u.RoleId = user.IsHouseOwner;

                        if (u.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)
                        {
                            house.OwnerID = u.Id;
                            _houseServices.Update(house);
                            foreach (
                                var userInHouse in
                                    house.Users.Where(
                                        usr => usr.Id != u.Id && u.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER))
                            {
                                User usr = _userServices.FindById(userInHouse.Id);
                                usr.RoleId = SLIM_CONFIG.USER_ROLE_RESIDENT;
                                usr.LastModified = DateTime.Now;
                                _userServices.Update(usr);
                            }
                        }
                        _userServices.Update(u);
                    }
                    else
                    {
                        response.StatusCode = -1;
                    }
                }
                else
                {
                    response.StatusCode = -1;
                }
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response);
            }

            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageUser/UpdateEmployee")]
        public ActionResult UpdateEmployee(UserInfoViewModel user)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                User u = _userServices.FindById(user.Id);
                if (null != u)
                {
                    u = parseUpdateDataUser(u, user);
                    _userServices.Update(u);
                }
                else
                {
                    response.StatusCode = -1;
                }
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageUser/UpdateManager")]
        public ActionResult UpdateManager(UserInfoViewModel user)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                User u = _userServices.FindById(user.Id);
                if (null != u)
                {
                    u = parseUpdateDataUser(u, user);
                    _userServices.Update(u);
                }
                else
                {
                    response.StatusCode = -1;
                }
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageUser/DeleteResident")]
        public ActionResult GetUserList(List<int> listResId)
        {
            MessageViewModels response = new MessageViewModels();
            if (null != listResId)
            {
                try
                {
                    foreach (var resId in listResId)
                    {
                        User u = _userServices.FindById(resId);
                        if (u.RoleId == SLIM_CONFIG.USER_ROLE_SUPPORTER)
                        {
                            if (u.HelpdeskRequests1.Any(r => r.Status == (int)StatusEnum.Processing))
                            {
                                response.StatusCode = 2;
                                response.Data = u.Fullname;
                                return Json(response);
                            }
                        }
                        if (u.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)
                        {
                            if (u.House.Users.Count != 1)
                            {
                                response.StatusCode = 4;
                                response.Data = new { houseName = u.House.HouseName, fullName = u.Fullname };
                                return Json(response);
                            }
                        }
                    }
                    foreach (var resId in listResId)
                    {
                        User u = _userServices.FindById(resId);
                        if (null != u)
                        {
                            if (u.RoleId == SLIM_CONFIG.USER_ROLE_SUPPORTER)// double check
                            {
                                if (u.HelpdeskRequests1.Any(r => r.Status == (int)StatusEnum.Processing))
                                {
                                    response.StatusCode = 2;
                                    response.Data = u.Fullname;
                                    return Json(response);
                                }
                            }
                            else if (u.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)// double check
                            {
                                if (u.House.Users.Count != 1)
                                {
                                    response.StatusCode = 4;
                                    response.Data = u.Fullname;
                                    response.Data = new { houseName = u.House.HouseName, fullName = u.Fullname };
                                    return Json(response);
                                }
                                House house = _houseServices.FindById(u.HouseId.Value);
                                house.OwnerID = null;
                                _houseServices.Update(house);
                            }
                            u.Status = SLIM_CONFIG.USER_STATUS_DELETE;
                            u.HouseId = null;
                            u.LastModified = DateTime.Now;
                            if (u.Posts != null)
                            {
                                foreach (var p in u.Posts)
                                {
                                    Post ePost = _postService.findPostById(p.Id);
                                    ePost.Status = SLIM_CONFIG.POST_STATUS_HIDE;
                                    ePost.UpdateDate = DateTime.Now;
                                    _postService.UpdatePost(ePost);
                                }
                            }
                            _userServices.Update(u);
                        }
                        else
                        {
                            response.StatusCode = -1;
                        }
                    }
                }
                catch (Exception e)
                {
                    response.StatusCode = -1;
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/ManageUser/DeleteManager")]
        public ActionResult DeleteManager(List<int> listResId)
        {
            MessageViewModels response = new MessageViewModels();
            if (null != listResId)
            {
                try
                {
                    foreach (var resId in listResId)
                    {
                        User u = _userServices.FindById(resId);
                        if (null != u)
                        {
                            u.Status = SLIM_CONFIG.USER_STATUS_DELETE;
                            u.HouseId = null;
                            u.LastModified = DateTime.Now;

                            if (u.Posts != null)
                            {
                                foreach (var p in u.Posts)
                                {
                                    Post ePost = _postService.findPostById(p.Id);
                                    ePost.Status = SLIM_CONFIG.POST_STATUS_HIDE;
                                    ePost.UpdateDate = DateTime.Now;
                                    _postService.UpdatePost(ePost);
                                }
                            }
                            _userServices.Update(u);
                        }
                        else
                        {
                            response.StatusCode = -1;
                        }
                    }
                }
                catch (Exception e)
                {
                    response.StatusCode = -1;
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/ManageUser/GetRoomAndFloor")]
        public ActionResult GetRoomAndFloor(int blockId, string floorName)
        {
            MessageViewModels response = new MessageViewModels();
            List<House> floor = _houseServices.GetAllFloorInBlock(blockId);
            List<string> floorStr = new List<string>();
            List<string> roomStr = new List<string>();
            List<string> roomIdStr = new List<string>();

            if (floor != null && floor.Count > 0)
            {
                foreach (var f in floor)
                {
                    floorStr.Add(f.Floor);
                }

                List<House> rooms = null;
                if (floorName.IsNullOrWhiteSpace())
                {
                    rooms = _houseServices.GetAllRoomsInFloor(blockId, floor[0].Floor);
                }
                else
                {
                    rooms = _houseServices.GetAllRoomsInFloor(blockId, floorName);
                }

                if (rooms != null && rooms.Count > 0)
                {
                    foreach (var room in rooms)
                    {
                        roomStr.Add(room.HouseName);
                        roomIdStr.Add(room.Id.ToString());
                    }
                }
                response.Data = new { Floor = floorStr, Room = roomStr, RoomId = roomIdStr };
            }
            else
            {
                response.Data = new { Floor = floorStr, Room = roomStr, RoomId = roomIdStr };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private User parseAddDataUser(UserInfoViewModel userInfo, int roleId)
        {
            Block block = _blockServices.FindByName(SLIM_CONFIG.EMPLOYEE_BLOCK_HOUSE_NAME);
            House house = null;
            if (block == null)
            {
                block = new Block();
                block.BlockName = SLIM_CONFIG.EMPLOYEE_BLOCK_HOUSE_NAME;
                _blockServices.Add(block);

                house = new House();
                house.BlockId = block.Id;
                house.Floor = SLIM_CONFIG.EMPLOYEE_BLOCK_HOUSE_NAME;
                house.Area = 0;
                house.Status = SLIM_CONFIG.HOUSE_STATUS_DISABLE;
                house.HouseName = SLIM_CONFIG.EMPLOYEE_BLOCK_HOUSE_NAME;
                _houseServices.Add(house);
            }
            else
            {
                house = block.Houses.Where(b => b.HouseName.Equals(SLIM_CONFIG.EMPLOYEE_BLOCK_HOUSE_NAME)).ToList().First();
            }

            User u = new User();
            u.Fullname = userInfo.Name;
            u.CreateDate = DateTime.Now;
            u.LastModified = DateTime.Now;
            u.IDNumber = userInfo.Idenity;
            u.Status = SLIM_CONFIG.USER_STATUS_ENABLE;
            u.Gender = userInfo.Gender;
            u.DateOfBirth = DateTime.ParseExact(userInfo.Dob, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
            u.IDCreatedDate = DateTime.ParseExact(userInfo.IdCreateDate, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
            u.Username = userInfo.UserAccountName;
            u.SendPasswordTo = userInfo.CellNumb;
            u.Creator = Int32.Parse(User.Identity.GetUserId());
            u.RoleId = roleId;
            u.HouseId = house.Id;
            u.Password = CommonUtil.GetUniqueKey(8);
            return u;
        }
        /*Reference /Home/getHintUsername
         */
        [HttpGet]
        [Route("Management/ManageUser/GetHintUsername")]
        public String GetHintUsername(String fullname, int? startNumber)
        {

            String hintResult;
            User testUser = null;
            hintResult = StringUtil.RemoveSign4VietnameseString(fullname).ToLower().Replace(" ", "");
            testUser = userService.findByUsername(hintResult + (startNumber == null ? "" : (startNumber + "")));
            if (testUser == null)
            {
                return hintResult + startNumber;
            }
            else
            {
                return GetHintUsername(hintResult, (startNumber == null ? 1 : startNumber + 1));
            }

        }
        [HttpGet]
        [Route("Management/ManageUser/CheckAvailableUsername")]
        public bool checkAvailableUsername(String username)
        {
            return userService.findByUsername(username) == null;

        }


        private User parseUpdateDataUser(User u, UserInfoViewModel userInfo)
        {
            u.Fullname = userInfo.Name;
            u.LastModified = DateTime.Now;
            u.IDNumber = userInfo.Idenity;
            u.Gender = userInfo.Gender;
            u.SendPasswordTo = userInfo.CellNumb;
            u.DateOfBirth = DateTime.ParseExact(userInfo.Dob, AmsConstants.DateFormat,
                CultureInfo.CurrentCulture);
            u.IDCreatedDate = DateTime.ParseExact(userInfo.IdCreateDate, AmsConstants.DateFormat, CultureInfo.CurrentCulture);
            u.Status = SLIM_CONFIG.USER_STATUS_ENABLE;
            return u;
        }
    }

}