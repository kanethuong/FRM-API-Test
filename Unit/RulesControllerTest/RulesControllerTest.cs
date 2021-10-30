using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.Controllers;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace kroniiapiTest.Unit.RulesControllerTest
{
    public class RulesControllerTest
    {
        public static readonly Mock<ICacheProvider> mockCacheProvider = new Mock<ICacheProvider>();
        public static readonly Mock<IMegaHelper> mockUploadHelper = new Mock<IMegaHelper>();
        public static readonly Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>();
        public static readonly RulesController rulesController = new RulesController(mockCacheProvider.Object, mockUploadHelper.Object, mockMemoryCache.Object);
    }
}