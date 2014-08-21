using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

// TODO: 将此项替换为要导入的类型。
using TImport = System.String;


namespace ContentPipelineExtension1
{
    /// <summary>
    /// 此类将由 XNA Framework 内容管道实例化，
    /// 以将磁盘文件导入为指定类型 TImport。
    /// 
    /// 这应当属于内容管道扩展库项目的一部分。
    /// 
    /// TODO: 更改 ContentImporter 属性以指定此导入程序的
    /// 正确文件扩展名、显示名和默认处理器。
    /// </summary>
    [ContentImporter(".rf", DisplayName = "RF Importer", DefaultProcessor = "RfContentProcessor")]
    public class RfFileContentImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            // TODO: 将指定的文件读入所导入类型的一个实例中。
            System.IO.StreamReader reader = new System.IO.StreamReader(filename);
            return reader.ReadToEnd();
        }
    }
}
