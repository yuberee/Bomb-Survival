﻿using GridAStar;
using System.Threading;
using System.Threading.Tasks;

namespace BombSurvival;

public partial class BombSurvival
{
	public static Grid MainGrid { get; private set; }
	public static Grid CachedGrid { get; private set; }
	public static float WorldWidth => 2048f;
	public static float WorldHeight => 2048f;
	public static BBox WorldBox => new BBox( new Vector3( -WorldWidth / 2f, -15f, -WorldWidth / 2f ), new Vector3( WorldWidth / 2f, 15f, WorldHeight / 2f ) );
	public static JumpDefinition LongJump => new JumpDefinition( "longJump", Player.BaseWalkSpeed, Player.JumpHeight * 1.35f, 2, maxPerCell: 1 );
	public static JumpDefinition HighJump => new JumpDefinition( "highJump", 60f, Player.JumpHeight * 1.35f, 2, maxPerCell: 1 );
	static int gridId = 0;

	public static async Task GenerateGrid()
	{
		CachedGrid?.Delete();
		CachedGrid = MainGrid;

		var builder = new GridAStar.GridBuilder( $"Grid{gridId}" )
			.WithBounds( Vector3.Zero, WorldBox, Rotation.Identity )
			.WithCellSize( Player.CollisionWidth )
			.WithHeightClearance( Player.CollisionHeight )
			.WithWidthClearance( Player.CollisionWidth )
			.WithEdgeNeighbourCount( 2 )
			.WithStepSize( Player.StepSize )
			.WithStandableAngle( Player.MaxWalkableAngle )
			.WithMaxDropHeight( 9999f )
			.AddJumpDefinition( LongJump )
			.AddJumpDefinition( HighJump )
			.JumpsIgnoreConnections( true )
			.JumpsIgnoreLOS( true );

		MainGrid = await builder.Create( 1, printInfo: false );

		await BombSurvivalBot.RecalculateAllPaths();
		CachedGrid?.Delete();

		gridId++;
	}

	[ConCmd.Admin( "bs_grid" )]
	static void GridDebug()
	{
		GenerateGrid().Wait();
	}
}