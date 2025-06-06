using System.Text;
using Lagrange.Core.Internal.Packets.Message.Element;
using Lagrange.Core.Internal.Packets.Message.Element.Implementation;
using Lagrange.Core.Utility.Binary.Compression;

namespace Lagrange.Core.Message.Entity;

[MessageElement(typeof(RichMsg))]
public class XmlEntity : IMessageEntity
{
    public string Xml { get; set; }

    public int ServiceId { get; set; } = 35;

    private static ReadOnlySpan<byte> Header => new byte[1] { 0x01 };

    public XmlEntity() => Xml = "";

    public XmlEntity(string xml) => Xml = xml;

    public XmlEntity(string xml, int serviceId) => (Xml, ServiceId) = (xml, serviceId);
    
    IEnumerable<Elem> IMessageEntity.PackElement()
    {
        return new Elem[]
        {
            new()
            {
                RichMsg = new RichMsg
                {
                    ServiceId = ServiceId,
                    Template1 = ZCompression.ZCompress(Xml, Header),
                }
            }
        };
    }
    
    IMessageEntity? IMessageEntity.UnpackElement(Elem elems)
    {
        if (elems.RichMsg is { ServiceId: 35, Template1: not null } richMsg)
        {
            var xml = ZCompression.ZDecompress(richMsg.Template1.AsSpan(1));
            return new XmlEntity(Encoding.UTF8.GetString(xml));
        }

        return null;
    }
    
    public string ToPreviewString() => $"[Xml]: {Xml}";
}