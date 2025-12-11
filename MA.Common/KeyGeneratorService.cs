// <copyright file="KeyGeneratorService.cs" company="Motion Applied Ltd.">
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

using MA.Common.Abstractions;

namespace MA.Common
{
    public class KeyGeneratorService : IKeyGeneratorService
    {
        private const string GeneratedKeyFileName = "key.info";
        private static readonly object LockObject = new();
        private static readonly object InitializationLockObject = new();
        private readonly bool canAccessFile;
        private readonly string filePath = GeneratedKeyFileName;
        private ulong lastGeneratedUlongId;
        private DateTime lastSnapshotTime = DateTime.UtcNow;

        public KeyGeneratorService(ILoggingDirectoryProvider loggingDirectoryProvider)
        {
            lock (InitializationLockObject)
            {
                if (this.lastGeneratedUlongId != 0)
                {
                    return;
                }

                this.filePath = Path.Combine(loggingDirectoryProvider.Provide(), GeneratedKeyFileName);
                if (!File.Exists(this.filePath))
                {
                    var snapshotTime = DateTime.UtcNow;
                    try
                    {
                        File.WriteAllText(this.filePath, snapshotTime.Ticks.ToString());
                        this.canAccessFile = true;
                    }
                    catch
                    {
                        Console.WriteLine("Can't Write Key.Info File");
                    }

                    this.lastSnapshotTime = snapshotTime;
                }

                if (!this.canAccessFile ||
                    !ulong.TryParse(File.ReadAllText(this.filePath), out this.lastGeneratedUlongId))
                {
                    this.GenerateUniqueUlongId();
                }
            }
        }

        public string GenerateStringKey()
        {
            return Guid.NewGuid().ToString();
        }

        public ulong GenerateUlongKey()
        {
            return this.GenerateUniqueUlongId();
        }

        private ulong GenerateUniqueUlongId()
        {
            lock (LockObject)
            {
                var dateTime = DateTime.UtcNow;
                var currentTicks = (ulong)dateTime.Ticks;
                if (currentTicks <= this.lastGeneratedUlongId)
                {
                    currentTicks = this.lastGeneratedUlongId + 1;
                }

                this.lastGeneratedUlongId = currentTicks;

                if (!this.canAccessFile ||
                    (this.lastSnapshotTime - dateTime).TotalMilliseconds <= 500)
                {
                    return currentTicks;
                }

                File.WriteAllText(this.filePath, this.lastGeneratedUlongId.ToString());
                this.lastSnapshotTime = dateTime;

                return currentTicks;
            }
        }
    }
}
