using System.Collections.ObjectModel;
using System.Linq;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            FieldViewModel lengthField;

            ClassViewModel chunkField;
            Root = new ClassViewModel("Png", new []
            {
                new FieldViewModel("FileHeader", "byte[]"),
                new CollectionViewModel("Chunks", "List<PngChunkContainer>", new []
                {
                    new ClassViewModel("PngChunkContainer", new []
                    {
                        lengthField = new FieldViewModel("Length", "int"),
                        new ClassViewModel("Payload", "PngChunkPayload", new []
                        {
                            new FieldViewModel("ChunkType", "string"),
                            chunkField = new ClassViewModel("Chunk", "PngChunk", Enumerable.Empty<FieldViewModel>(), new []
                            {
                                new ClassViewModel("PngImageDataChunk", new []
                                {
                                    new FieldViewModel("Data", "byte[]"), 
                                }), 
                                new ClassViewModel("PngImageHeaderChunk", new []
                                {
                                    new FieldViewModel("Data", "byte[]"),
                                }),
                            })
                        }),
                        new FieldViewModel("Crc", "uint")
                    }), 
                })
            });

            lengthField.Bindings.Add(new BindingViewModel(lengthField, chunkField));
        }

        public ClassViewModel Root { get; set; }
    }
}
