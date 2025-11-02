using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Models.Context;
using BackEndService.Workflows.Modules.OrderProcessing;
using Moq;
using Xunit;

namespace BackEndService.UnitTests
{
    public class STTWorkflowModuleTests
    {
        [Fact]
        public async Task ExecuteAsync_WithAudioStream_ReturnsTranscription()
        {
            var mockStt = new Mock<BackEndService.Core.Interfaces.Services.ISTTService>();
            mockStt.Setup(x => x.TranscribeAsync(It.IsAny<Stream>())).ReturnsAsync("hello world");
            
            var module = new BackEndService.Workflows.Modules.STT.STTWorkflowModule(mockStt.Object);
            var context = new WorkflowContext { UserId = "test" };
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("audio"));
            
            var result = await module.ExecuteAsync(context, stream);
            
            Assert.NotNull(result);
            // Verify the transcription was called
            mockStt.Verify(x => x.TranscribeAsync(It.IsAny<Stream>()), Times.Once);
        }
    }

    public class TTSWorkflowModuleTests
    {
        [Fact]
        public async Task ExecuteAsync_WithText_ReturnsAudioLength()
        {
            var mockTts = new Mock<BackEndService.Core.Interfaces.Services.ITTSService>();
            using var audioStream = new MemoryStream(Encoding.UTF8.GetBytes("audio data"));
            mockTts.Setup(x => x.SynthesizeAsync("test")).ReturnsAsync(audioStream);
            
            var module = new BackEndService.Workflows.Modules.TTS.TTSWorkflowModule(mockTts.Object);
            var context = new WorkflowContext { UserId = "test" };
            
            var result = await module.ExecuteAsync(context, "test");
            
            Assert.NotNull(result);
            mockTts.Verify(x => x.SynthesizeAsync("test"), Times.Once);
        }
    }

    public class RAGWorkflowModuleTests
    {
        [Fact]
        public async Task ExecuteAsync_WithQuery_ReturnsResults()
        {
            var mockRag = new Mock<BackEndService.Core.Interfaces.Services.IRAGService>();
            mockRag.Setup(x => x.SearchAsync("pizza", "test")).ReturnsAsync(new[] { "result1", "result2" });
            
            var module = new BackEndService.Workflows.Modules.RAG.RAGWorkflowModule(mockRag.Object);
            var context = new WorkflowContext { UserId = "test" };
            
            var result = await module.ExecuteAsync(context, "pizza");
            
            Assert.NotNull(result);
            mockRag.Verify(x => x.SearchAsync("pizza", "test"), Times.Once);
        }
    }

    public class OrderWorkflowModuleTests
    {
        [Fact]
        public async Task ExecuteAsync_WithArguments_ReturnsOrderStatus()
        {
            var module = new OrderWorkflowModule();
            var context = new WorkflowContext { UserId = "test", PaymentId = "pay_123" };
            var args = JsonDocument.Parse("{\"items\":[{\"sku\":\"pizza\",\"qty\":1}]}");
            
            var result = await module.ExecuteAsync(context, args.RootElement);
            
            Assert.NotNull(result);
        }
    }
}

