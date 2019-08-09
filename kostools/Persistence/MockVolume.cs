using System;
using kOS.Safe;
using kOS.Safe.Persistence;

namespace kOS.Tools.Persistence
{
    class MockVolume : kOS.Safe.Persistence.Volume
    {
        public override VolumeDirectory Root => throw new NotImplementedException();

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override VolumeDirectory CreateDirectory(VolumePath path)
        {
            throw new NotImplementedException();
        }

        public override VolumeFile CreateFile(VolumePath path)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(VolumePath path, bool ksmDefault = false)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(VolumePath path, bool ksmDefault = false)
        {
            throw new NotImplementedException();
        }

        public override VolumeItem Open(VolumePath path, bool ksmDefault = false)
        {
            throw new NotImplementedException();
        }

        public override VolumeFile SaveFile(VolumePath path, FileContent content, bool verifyFreeSpace = true)
        {
            throw new NotImplementedException();
        }
    }
}
