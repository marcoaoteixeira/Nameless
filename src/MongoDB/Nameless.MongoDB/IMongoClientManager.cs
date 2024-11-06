using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace Nameless.MongoDB;
public interface IMongoClientManager {
    IMongoClient GetMongoClient();
}
