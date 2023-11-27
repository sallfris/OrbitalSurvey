﻿using KSP.Sim;
using KSP.Sim.impl;

namespace OrbitalSurvey.Utilities;

public static class OrbitUtility
{
    public static Vector3d WorldPositionAtUT(this PatchedConicsOrbit o, double UT)
    {
        return o.referenceBody.transform.celestialFrame.ToLocalPosition(o.ReferenceFrame, o.referenceBody.Position.localPosition + o.GetRelativePositionAtUTZup(UT).SwapYAndZ);
    }
    
    public static void GetOrbitalParametersAtUT(VesselComponent vessel, double UT, out double latitude, out double longitude, out double altitude)
    {
        Position position = new Position(vessel.Orbit.referenceBody.coordinateSystem, vessel.Orbit.WorldPositionAtUT(UT));
        vessel.Orbit.referenceBody.GetLatLonAltFromRadius(position, out latitude, out longitude, out altitude);
        
        longitude += GetLongitudeOffsetDueToRotationForAGivenUT(vessel.Orbit.referenceBody, UT);

        if (longitude < -180f)
            longitude += 360f;

        // directRotAngle = 254.234223135939
        // rotationAngle = 254.234223135939
        // rotationPeriod = 21549.425 (in seconds?)

        // sphereOfInfluence = 84159286.4796305
        // radius = 600000
    }

    public static double GetLongitudeOffsetDueToRotationForAGivenUT(CelestialBodyComponent body, double UT)
    {
        // C (circumference) = 2rπ
        // length of day = time it takes for a 1 full rotation, i.e. C)
        // dt = delta T from now to the given UT
        // latitude difference = (horizontal distance / radius of the planet) * (180 / π)
        
        var circumference = 2 * body.radius * Math.PI;
        var lengthOfDay = body.rotationPeriod;
        var deltaUT = ScanUtility.UT - UT;
        var rotationDifferenceAtEquator = (deltaUT * circumference) / lengthOfDay;
        
        return (rotationDifferenceAtEquator / body.radius) * (180 / Math.PI);
    }
}