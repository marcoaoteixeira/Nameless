using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Nameless.Data.UnitTesting {
    public class ParameteTests {

        [Test]
        public void Create_A_Generic_Parameter_To_Deal_With_Database() {
            // arrange
            Parameter input;
            Parameter output;
            Parameter retVal;

            // act
            input = Parameter.CreateInputParameter("TEST");

            // assert
            Assert.Multiple(() => {
                input.Should().NotBeNull();
            });
        }
    }
}
