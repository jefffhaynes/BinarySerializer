using System.Linq;
using BinarySerialization.Graph;
using BinarySerializer.Editor.Test;

namespace BinarySerializer.Editor.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            //FieldViewModel lengthField;
            //FieldViewModel chunkTypeField;
            //ClassViewModel payloadField;
            //ClassViewModel chunkField;
            //FieldViewModel crcField;
            //Root = new ClassViewModel("Png", new []
            //{
            //    new FieldViewModel("FileHeader", "byte[]"),
            //    new CollectionViewModel("Chunks", "List<PngChunkContainer>", new []
            //    {
            //        new ClassViewModel("PngChunkContainer", new []
            //        {
            //            lengthField = new FieldViewModel("Length", "int"),
            //            payloadField = new ClassViewModel("Payload", "PngChunkPayload", new []
            //            {
            //                chunkTypeField = new FieldViewModel("ChunkType", "string"),
            //                chunkField = new ClassViewModel("Chunk", "PngChunk", Enumerable.Empty<FieldViewModel>(), new []
            //                {
            //                    new ClassViewModel("PngImageDataChunk", new []
            //                    {
            //                        new FieldViewModel("Data", "byte[]"), 
            //                    }), 
            //                    new ClassViewModel("PngImageHeaderChunk", new []
            //                    {
            //                        new FieldViewModel("Data", "byte[]"),
            //                    }),
            //                })
            //            }),
            //            crcField = new FieldViewModel("Crc", "uint")
            //        }), 
            //    })
            //});

            //lengthField.Bindings.Add(new BindingViewModel(lengthField, chunkField));
            //chunkTypeField.Bindings.Add(new BindingViewModel(chunkTypeField, chunkField));
            //crcField.Bindings.Add(new BindingViewModel(crcField, payloadField));

            var generator = new GraphGenerator();
            var graph = generator.GenerateGraph(typeof(Png));

            var viewModelGenerator = new ViewModelGenerator();
            Root = viewModelGenerator.Generate(graph) as ClassViewModel;
        }

        public ClassViewModel Root { get; set; }

        
    }
}
