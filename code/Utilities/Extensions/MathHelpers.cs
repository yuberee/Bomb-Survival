﻿namespace BombSurvival;

public static class MathHelpers
{
	public static float Map( this float input, float inputMin, float inputMax, float min, float max )
	{
		return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
	}

}
