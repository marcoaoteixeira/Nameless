namespace Nameless.AspNetCore.UnitTest {

    public class HashUtilTests {

        [Test]
        public void Hash_Should_Return_Hashed_Value() {
            // arrange
            var value = "123456abc@";

            // act
            var actual = HashUtil.Hash(value);

            // assert
            Assert.That(actual, Is.Not.EqualTo(value));
        }

        [Test]
        public void Validate_Should_Return_True_When_Value_Is_Correct_Unhashed_Value() {
            // arrange
            var hash = "xCsdlRlU+ycB+Z1+rtJivAJVewIlvILQyKXQICjPyPXYBubHQ0f9TcyjZuqG+EH08ZajhX1i3zae6+T/qMqAjZR/3s9j5NHffgnJgjU4YigLddHdkp4oTuRvDJxJiUXLiGUTcm0twkevaXFLeSKnekpM/xfhOPq8oAb5B3lkjFJswHGbt5VcT0Cwu0CbMljCA5nD9i+r3n4D7GK5eDzLV++vIno6C7653HSca72ZyF9AGWcqiItoMD9ROe23H3fdCDpoNgEsALaKF8yoP1G4RuPGU7j1PCZwRb1olZkLm+YjE8Lyo/edG4msDRDXVw//DlS0M0m5vprUBWnFOtN/MsRjQXWgtLXCtIT3E2iCAwpiGdjh0MqB19/Dma9wpe0doHbbjQT46Lq1ZA4GenNdz9RQM4mcI4Td5pwMgeMwdGc=";
            var value = "123456abc@";

            // act
            var actual = HashUtil.Validate(value, hash);

            // assert
            Assert.That(actual, Is.True);
        }
    }
}