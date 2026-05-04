using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipURL.Services.ShorterURL.Helpers;

namespace ZipURL.Tests
{
    public class HashidHelperTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999999)]
        public void EncodeAndDecode_ShouldReturnOriginalId(int originalId)
        {
            // Act: Mã hóa ID thành chuỗi
            string code = HashidHelper.Encode(originalId);

            // Act: Giải mã chuỗi ngược lại thành ID
            int decodedId = HashidHelper.Decode(code);

            // Assert: Kiểm tra xem ID sau khi giải mã có bằng ID ban đầu không
            Assert.Equal(originalId, decodedId);
            Assert.NotNull(code);
            Assert.NotEmpty(code);
        }

        [Fact]
        public void Decode_ValidCode_ShouldReturnCorrectId()
        {
            // Arrange: Chuẩn bị một ID thực tế (ví dụ ID từ Database)
            int expectedId = 2026;

            // Tạo ra một mã hợp lệ bằng chính hàm Encode của mình
            string validCode = HashidHelper.Encode(expectedId);

            // Act: Tiến hành giải mã mã vừa tạo
            int actualId = HashidHelper.Decode(validCode);

            // Assert: Kiểm tra kết quả
            Assert.Equal(expectedId, actualId); // ID sau giải mã phải khớp với ID ban đầu
            Assert.True(validCode.Length >= 6); // Kiểm tra độ dài tối thiểu đã cấu hình (MinHashLength = 6)
        }
    }
}
