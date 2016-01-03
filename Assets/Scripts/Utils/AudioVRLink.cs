using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class AudioVRLink {
    private float[] rotationMatrix;

    public float[] getRotationMatrix() {
        return rotationMatrix;
    }

    internal static void connect() {
        throw new NotImplementedException();
    }

    internal static void disconnect() {
        throw new NotImplementedException();
    }

    internal static bool isConnected()
    {
        return false;
    }
}
