#pragma strict




function startWebXml(param : String)
{
		gameObject.SendMessage("passWebXml",param);
}



function resetWebPlayer(param: String)
{

 gameObject.SendMessage("resetPlayer",param);

}