using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: 将这些项替换为处理器输入和输出类型。
using TInput = System.String;
using TOutput = System.String;

namespace ContentPipelineExtension1
{
    /// <summary>
    /// 此类将由 XNA Framework 内容管道实例化，
    /// 以便将自定义处理应用于内容数据，将对象转换用于类型 TInput 到
    /// 类型 TOutput 的改变。如果处理器希望改变数据但不更改其类型，
    /// 输入和输出类型可以相同。
    ///
    /// 这应当属于内容管道扩展库项目的一部分。
    ///
    /// TODO: 更改 ContentProcessor 属性以为此处理器指定
    /// 正确的显示名称。
    /// </summary>
    [ContentProcessor(DisplayName = "Rf Processor")]
    public class RfContentProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            // TODO: 处理输入对象，并返回修改的数据。
            return input;
        }
    }
}
