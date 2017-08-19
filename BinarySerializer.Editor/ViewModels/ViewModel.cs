using System.Linq;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            Root = new ObjectViewModel("Png", new []
            {
                new FieldViewModel("FileHeader"),
                new CollectionViewModel("Chunks", new []
                {
                    new ObjectViewModel("PngChunkContainer", new []
                    {
                        new FieldViewModel("Length"),
                        new ObjectViewModel("Payload", new []
                        {
                            new FieldViewModel("ChunkType"),
                            new ObjectViewModel("Chunk", Enumerable.Empty<FieldViewModel>())
                        }),
                        new FieldViewModel("Crc"), 
                    }), 
                })
            });
        }

        public ObjectViewModel Root { get; set; }
    }
}
