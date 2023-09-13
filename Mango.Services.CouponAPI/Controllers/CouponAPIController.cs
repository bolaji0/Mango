using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _responseDTO;
        private IMapper _mapper;

        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _responseDTO = new ResponseDTO();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDTO Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons;
                _responseDTO.Result = _mapper.Map<IEnumerable<CouponDTO>>(objList);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO Get(int id)
        {
            try
            {
                Coupon objList = _db.Coupons.First(x => x.CouponId == id);
                _responseDTO.Result = _mapper.Map<CouponDTO>(objList);
            }
            catch (Exception)
            {
                _responseDTO.IsSuccess = true;
                _responseDTO.Message = "";
            }
            return _responseDTO;
        }

        [HttpGet]
        [Route("GetByCode/{Code}")]
        public ResponseDTO GetByCode(string Code)
        {
            try
            {
                var getCode = _db.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == Code.ToLower());
                if (getCode == null)
                {
                    _responseDTO.IsSuccess = false;
                }
                _responseDTO.Result = _mapper.Map<CouponDTO>(getCode);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPost]
        public ResponseDTO Post([FromBody] CouponDTO coupon)
        {
            try
            {
                Coupon convertToCoupon = _mapper.Map<Coupon>(coupon);
                _db.Coupons.Add(convertToCoupon);
                _db.SaveChanges();
                _responseDTO.Result = _mapper.Map<CouponDTO>(convertToCoupon);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPut]
        public ResponseDTO put([FromBody] CouponDTO coupon)
        {
            try
            {
                Coupon convertToCoupon = _mapper.Map<Coupon>(coupon);
                _db.Coupons.Update(convertToCoupon);
                _db.SaveChanges();
                _responseDTO.Result = _mapper.Map<CouponDTO>(convertToCoupon);

            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDTO delete(int id)
        {
            try
            {
                Coupon getId = _db.Coupons.FirstOrDefault(x => x.CouponId == id);
                _db.Coupons.Remove(getId);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        } 

    }
}
