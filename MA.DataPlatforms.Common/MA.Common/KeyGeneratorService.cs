// <copyright file="KeyGeneratorService.cs" company="McLaren Applied Ltd.">
//
// Copyright 2024 McLaren Applied Ltd
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
        private const string GeneratedKeyFilePath = "key.info";
        private static readonly object LockObject = new();
        private static readonly object InitializationLockObject = new();
        private ulong lastGeneratedUlongId;
        private DateTime lastSnapshotTime = DateTime.UtcNow;

        public KeyGeneratorService()
        {
            lock (InitializationLockObject)
            {
                if (this.lastGeneratedUlongId != 0)
                {
                    return;
                }

                if (!File.Exists(GeneratedKeyFilePath))
                {
                    var snapshotTime = DateTime.UtcNow;
                    File.WriteAllText(GeneratedKeyFilePath, snapshotTime.Ticks.ToString());
                    this.lastSnapshotTime = snapshotTime;
                }

                if (!ulong.TryParse(File.ReadAllText(GeneratedKeyFilePath), out this.lastGeneratedUlongId))
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

                if ((this.lastSnapshotTime - dateTime).TotalMilliseconds > 500)
                {
                    File.WriteAllText(GeneratedKeyFilePath, this.lastGeneratedUlongId.ToString());
                    this.lastSnapshotTime = dateTime;
                }

                return currentTicks;
            }
        }
    }
}
