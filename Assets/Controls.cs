using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BGI/Controls")]
public class Controls : ScriptableObject
{
	public KeyCode Throw = KeyCode.Mouse0;
	public KeyCode Attack = KeyCode.Mouse1;
	public KeyCode Rally = KeyCode.Mouse2;


	public KeyCode Build = KeyCode.Mouse0;
	public KeyCode Break = KeyCode.Mouse1;

	public KeyCode Interact = KeyCode.E;
	public KeyCode Call = KeyCode.R;
	public KeyCode Dismiss = KeyCode.T;
	public KeyCode Mark = KeyCode.Q;

	public KeyCode CameraLock = KeyCode.Tab;


	public KeyCode MoveUp = KeyCode.W;
	public KeyCode MoveDown = KeyCode.S;
	public KeyCode MoveLeft = KeyCode.A;
	public KeyCode MoveRight = KeyCode.D;
	public KeyCode Jump = KeyCode.Space;

	
	public KeyCode MoveUpAlt = KeyCode.UpArrow;
	public KeyCode MoveDownAlt = KeyCode.DownArrow;
	public KeyCode MoveLeftAlt = KeyCode.LeftArrow;
	public KeyCode MoveRightAlt = KeyCode.RightArrow;


	public KeyCode BuildMC= KeyCode.U;


	public KeyCode BreakMC = KeyCode.K;

	public KeyCode BuildmodeMC = KeyCode.J;

    public KeyCode CycleBlockUp = KeyCode.Plus;
    public KeyCode CycleBlockDown = KeyCode.Minus;

	public KeyCode TransRights = KeyCode.F13;


}



public static class InMan
{
	public static Controls Controls => Default.I.controls;
	public static bool Throw => Input.GetKeyDown(Controls.Throw);
	public static bool Summon => Input.GetKeyDown(Controls.Interact);
	public static bool Fireball => Input.GetKeyDown(Controls.Attack);
	public static bool Harvest => Input.GetKeyDown(Controls.Interact);
	public static bool Melee => false; // ha you want rights.



	public static bool BreakMC => Input.GetKeyDown(Controls.BreakMC);
	public static bool BuildMC => Input.GetKeyDown(Controls.BuildMC);
	public static bool BuildmodeMC => Input.GetKeyDown(Controls.BuildmodeMC);
    public static bool ChangeBlockUp => Input.GetKeyDown(Controls.CycleBlockUp);
    public static bool ChangeBlockDown => Input.GetKeyDown(Controls.CycleBlockDown);

	public static bool Build => Input.GetKeyDown(Controls.Build);
	public static bool Break => Input.GetKeyDown(Controls.Break);
	public static bool Kill => Input.GetKeyDown(Controls.Mark);
	public static bool Rally => Input.GetKey(Controls.Rally);
	public static bool Whistle => Input.GetKey(Controls.Call);
	public static bool CameraLock => Input.GetKeyDown(Controls.CameraLock);
}


static class English {

static readonly string[] CommonNouns = @"time year people way day man thing woman life child world school state family student group country problem hand part place case week company system program question work government number night point home water room mother area money story fact month lot right study book eye job word business issue side kind head house service friend father power hour game line end member law car city community name president team minute idea kid body information back parent face others level office door health person art war history party result change morning reason research girl guy moment air teacher force education".Split(' ');
static readonly string[] CommonAdjectives = "other new good high old great big American small large national young different black long little important political bad white real best right social only public sure low early able human local late hard major better economic strong possible whole free military true federal international full special easy clear recent certain personal open red difficult available likely short single medical current wrong private past foreign fine common poor natural significant similar hot dead central happy serious ready simple left physical general environmental financial blue democratic dark various entire close legal religious cold final main green nice huge popular traditional cultural".Split(' ');

	public static string RandomName()
	{
		return CommonAdjectives.Random() + CommonNouns.Random();
	}

}