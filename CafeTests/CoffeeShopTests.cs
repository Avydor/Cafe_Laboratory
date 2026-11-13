using Xunit;
using CafeSimulation.Models;

namespace CafeTests
{
    public class VipCustomerTests
    {
        [Fact]
        public void CalculatePrice_ShouldApplyVipDiscountCorrectly()
        {
            // Arrange (Підготовка)
            var vipCustomer = new VipCustomer("Олександр", 1000m);
            decimal originalPrice = 200.0m;

            // Act (Дія)
            decimal finalPrice = vipCustomer.CalculatePrice(originalPrice);

            // Assert (Перевірка: 200 - 10% = 180)
            Assert.Equal(180.0m, finalPrice);
        }

        [Fact]
        public void CalculatePrice_ShouldHandleZeroPrice()
        {
            // Arrange (Граничний випадок: нульова ціна)
            var vipCustomer = new VipCustomer("Марія", 500m);
            decimal originalPrice = 0.0m;

            // Act
            decimal finalPrice = vipCustomer.CalculatePrice(originalPrice);

            // Assert
            Assert.Equal(0.0m, finalPrice);
        }

        [Fact]
        public void VipCustomer_ShouldStoreCustomDiscountCorrectly()
        {
            // Arrange & Act (Перевірка зміни стандартної знижки на іншу, наприклад 20%)
            var vipCustomer = new VipCustomer("Олег", 1000m)
            {
                Discount = 0.20m
            };

            decimal finalPrice = vipCustomer.CalculatePrice(200.0m);

            // Assert (200 - 20% = 160)
            Assert.Equal(160.0m, finalPrice);
        }

        [Theory]
        [InlineData(100.0, 0.10, 90.0)]
        [InlineData(500.0, 0.20, 400.0)]
        [InlineData(50.0, 0.0, 50.0)]
        public void CalculatePrice_WithVariousDiscounts_ShouldCalculateCorrectly(decimal price, decimal discount, decimal expected)
        {
            // Arrange (Використання [Theory] дозволяє перевірити одразу кілька варіантів вхідних даних)
            var vipCustomer = new VipCustomer("Тест", 1000m)
            {
                Discount = discount
            };

            // Act
            decimal result = vipCustomer.CalculatePrice(price);

            // Assert
            Assert.Equal(expected, result);
        }
    }

    public class CafeModelsTests
    {
        [Fact]
        public void Customer_ShouldInitializeCorrectly_AndCalculateBasePrice()
        {
            // Arrange
            var customer = new Customer("Іван", 500.0m);
            decimal originalPrice = 100.0m;

            // Act
            decimal finalPrice = customer.CalculatePrice(originalPrice);

            // Assert
            Assert.Equal("Іван", customer.Name);
            Assert.Equal(500.0m, customer.Balance);
            Assert.Equal(originalPrice, finalPrice);
        }

        [Fact]
        public void CoffeeItem_ShouldStorePropertiesCorrectly()
        {
            // Arrange & Act
            var coffee = new CoffeeItem(1, "Espresso", 45.0m, "Опис", default);

            // Assert
            Assert.Equal("Espresso", coffee.Name);
            Assert.Equal(45.0m, coffee.Price);
        }

        [Fact]
        public void BakeryItem_ShouldStorePropertiesCorrectly()
        {
            // Arrange & Act
            var bakery = new BakeryItem(1, "Croissant", 60.0m, "Опис", true);

            // Assert
            Assert.Equal("Croissant", bakery.Name);
            Assert.Equal(60.0m, bakery.Price);
        }
    }
}