// <copyright file="ConsoleLogger.cs" company="Motion Applied Ltd.">
//
// Copyright 2025 Motion Applied Ltd
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Diagnostics.CodeAnalysis;

using MA.Common.Abstractions;

namespace MA.Common;

[ExcludeFromCodeCoverage]
public class ConsoleLogger : ILogger
{
    private readonly object @lock = new();

    public void Debug(string message)
    {
        this.WriteInConsole(ConsoleColor.Yellow, nameof(this.Debug), message);
    }

    public void Error(string message)
    {
        this.WriteInConsole(ConsoleColor.Red, nameof(this.Error), message);
    }

    public void Info(string message)
    {
        this.WriteInConsole(ConsoleColor.Green, nameof(this.Info), message);
    }

    public void Trace(string message)
    {
        this.WriteInConsole(ConsoleColor.Blue, nameof(this.Trace), message);
    }

    public void Warning(string message)
    {
        this.WriteInConsole(ConsoleColor.Magenta, nameof(this.Warning), message);
    }

    private void WriteInConsole(ConsoleColor color, string type, string message)
    {
        lock (this.@lock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{type}: " + message);
            Console.ResetColor();
        }
    }
}
