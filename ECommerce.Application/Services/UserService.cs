using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;


namespace ECommerce.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // --- MAPPING HELPER ---
        private UserDTO MapToDto(UserEntity user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address,
                Enabled = user.Enabled,
                IsSellerApproved = user.IsSellerApproved,
                HasPendingSellerRequest = user.HasPendingSellerRequest,
                IsRejected = user.IsRejected,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
                CreatedAt = user.CreatedAt,

                // --- SİPARİŞLERİ MAPLE ---
                Orders = user.Orders?.Select(o => new OrderDTO
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    PaymentMethod = o.PaymentMethod,
                    // Teslimat bilgilerini Entity'den DTO'ya aktar
                    DeliveryAddress = o.DeliveryAddress,
                    DeliveryFullName = o.DeliveryFullName,
                    DeliveryPhone = o.DeliveryPhone,
                    UserId = o.UserId,
                    UserFullName = $"{user.FirstName} {user.LastName}",

                    Items = o.OrderItems?.Select(oi => new OrderItemDTO
                    {
                        ProductId = oi.ProductId, // oi.Id değil, ProductId olmalı
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        // Görsel bilgisini al (Eğer Product Include edilmişse)
                        ImageUrl = oi.Product?.Images?.FirstOrDefault(x => x.IsMain)?.Url
                                   ?? oi.Product?.Images?.FirstOrDefault()?.Url
                                   ?? "default.jpg"
                    }).ToList() ?? new List<OrderItemDTO>()
                }).ToList() ?? new List<OrderDTO>(),

                // --- ÜRÜNLERİ MAPLE (Burada 'Items' değil 'Products' olmalı) ---
                Products = user.Products?.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    StockAmount = p.StockAmount,
                    Enabled = p.Enabled,
                    IsFeatured = p.IsFeatured,
                    CategoryName = p.Category?.Name ?? "General",
                    MainImageUrl = p.Images?.FirstOrDefault(x => x.IsMain)?.Url
                                   ?? p.Images?.FirstOrDefault()?.Url
                                   ?? "default.jpg",
                    // --- Yorumları DTO'ya aktar ---
                    Comments = p.Comments?.Select(c => new ProductCommentDTO
                    {
                        Id = c.Id,
                        UserName = c.User?.FirstName + " " + c.User?.LastName, 
                        Text = c.Text,
                        StarCount = c.StarCount,
                        IsConfirmed = c.IsConfirmed,
                        CreatedAt = c.CreatedAt
                    }).ToList() ?? new List<ProductCommentDTO>()
                }).ToList() ?? new List<ProductDTO>()
            };

            }

        // Bütün kullanıcıları al
        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllWithRolesAsync();
            return users.Select(MapToDto);
        }

        // ID ile kullanıcıyı al
        public async Task<UserDTO?> GetAsync(int id, bool includeOrders = false, bool includeProducts = false)
        {
            var user = await _userRepository.GetByIdWithIncludesAsync(id, includeOrders, includeProducts);
            return user != null ? MapToDto(user) : null;
        }

        // Kullanıcıyı ekle 
        //Aslında şu an registerla kayıt oluyor ama ileride admine ekleme yetkisi verilirse kullanılabilir
        //Aksi halde register'a da yönlendirilebilir.
        public async Task AddAsync(UserDTO dto)
        {
            var user = new UserEntity
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = dto.Password,
                RoleId = dto.RoleId, // Admin hangi rolü seçtiyse o (Örn: Seller)
                Enabled = dto.Enabled,
                CreatedAt = DateTime.Now,
                Phone = dto.Phone,
                Address = dto.Address
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();
        }

        // Kullanıcıyı güncelle 
        public async Task UpdateAsync(UserDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user != null)
            {
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Phone = dto.Phone;
                user.Address = dto.Address;
                
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        // Kullanıcıyı sil 
        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                if (user.Role?.Name == "SystemAdmin")
                {
                    throw new InvalidOperationException("The System Administrator account cannot be deleted.");
                }

                await _userRepository.DeleteAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        // Seller onayla
        public async Task ApproveSellerAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsSellerApproved = true;
                user.HasPendingSellerRequest = false;

                var sellerRole = await _roleRepository.GetByNameAsync("Seller");
                if (sellerRole != null)
                {
                    user.RoleId = sellerRole.Id;
                }

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }


        // Email ile kullanıcı getirme (async)
        public async Task<UserDTO?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }


        // Kullanıcı kaydetme (async)
        public async Task<UserDTO> RegisterAsync(UserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new Exception("Password is required for the registration.");
            }
            var user = new UserEntity
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = dto.Password, // İleride Hash eklenebilir
                RoleId = 3, // Default: Buyer
                Enabled = true,
                CreatedAt = DateTime.Now,
                Phone = dto.Phone,
                Address = dto.Address
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            dto.Id = user.Id;
            return dto;
        }



        // Authenticate
        public async Task<LoginResponseDTO?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user != null && user.Password == password)
            {
                // Interface bizden LoginResponseDTO bekliyor, paketi hazırlıyoruz:
                return new LoginResponseDTO
                {
                    // Şimdilik test için sabit string, normalde JWT üretilmeli
                    Token = "GENERATE_JWT_TOKEN_HERE",
                    User = MapToDto(user)
                };
            }
            return null;
        }



        // ------------------- REQUEST SELLER STATUS -------------------
        // API tarafındaki UserService.cs
        public async Task RequestSellerStatusAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsSellerApproved || user.HasPendingSellerRequest) return;

            user.HasPendingSellerRequest = true;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();
        }

        public async Task RequestSellerStatusAsync()
        {
            // API tarafında bu metod genellikle doğrudan çağrılmaz.
            // Eğer çağrılırsa hata fırlatabilir veya boş bırakılabilir.
            throw new NotImplementedException("Methods with IDs should be used on the API side.");
        }

        //--------------TOGGLE-------------------
        public async Task ToggleEnabledAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                // SystemAdmin hesabı dondurulamaz!
                if (user.Role?.Name == "SystemAdmin")
                {
                   throw new InvalidOperationException("The System Administrator account is protected and cannot be deactivated.");
                }

                user.Enabled = !user.Enabled;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        public async Task RejectSellerAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.HasPendingSellerRequest = false; // Bekleyen isteği kaldır
                user.IsRejected = true;                // Reddedildi olarak işaretle
                user.IsSellerApproved = false;         // Onaylı değil

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
            }
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            // 1. Güvenli ve benzersiz bir token oluştur
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.TokenExpiresAt = DateTime.Now.AddHours(2); // 2 saat geçerli

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            // 2. Burada EmailService üzerinden mail göndereceğiz
            // Örn: _emailService.SendResetMail(user.Email, user.ResetToken);

            return true;
        }

        public async Task<bool> VerifyResetTokenAsync(string token)
        {
            var user = await _userRepository.GetByResetTokenAsync(token);

            // Kullanıcı var mı ve token süresi dolmamış mı?
            if (user != null && user.TokenExpiresAt > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            // 1. Token'a sahip kullanıcıyı bul (UserEntity'de ResetToken alanı olmalı)
            var user = await _userRepository.GetByResetTokenAsync(token); // Repository'e bu metod eklenmeli!!!

            if (user == null || user.TokenExpiresAt < DateTime.Now)
                return false;

            // 2. Şifreyi güncelle (Normalde Hashlenmeli!)
            user.Password = newPassword;
            user.PasswordResetToken = null; // Token'ı tek kullanımlık yap
            user.TokenExpiresAt = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<UserDTO?> GetCurrentUserAsync(int userId, bool includeOrders = false, bool includeProducts = false)
        {
            return await GetAsync(userId, includeOrders, includeProducts);
        }

        public async Task UpdateCurrentUserAsync(UserDTO dto)
        {
            await UpdateAsync(dto); // Mevcut UpdateAsync metodu kullanılabilir
        }
    }
}