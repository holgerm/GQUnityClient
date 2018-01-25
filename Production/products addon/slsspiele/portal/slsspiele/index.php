<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>


<?php




if(isset($_GET['content'])) {
$url = $_GET['content'];

} else {
	
	?>
	<script>
	
	  	  window.location.replace("http://qeevee.org:9091/421/login");

  
</script>
	<?


	
die();
	
	
}



?>

<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<title>WikiCultureCity <? if($url == "template"){ ?>| %%_GEOQUEST_CONTENT_TITLE_%%<? } ?></title>
<style type="text/css">
html,body {
    height:100%;
}
body,td,th {
	color: #000 !important;
	font-family: "Trebuchet MS", Arial, Helvetica, sans-serif !important;
}
a:link {
	text-decoration: underline !important;
	color:#000 !important;
}
a:visited {
	text-decoration: underline !important;
		color:#000 !important;

}
a:hover {
	text-decoration: none !important;
		color:#000 !important;

}
a:active {
	text-decoration: underline !important;
		color:#000 !important;

}


.btn-success {
 background-color: hsl(44, 100%, 70%) !important;
  background-repeat: repeat-x!important;
  filter: progid:DXImageTransform.Microsoft.gradient(startColorstr="#fff6e0", endColorstr="#ffd665")!important;
  background-image: -khtml-gradient(linear, left top, left bottom, from(#fff6e0), to(#ffd665))!important;
  background-image: -moz-linear-gradient(top, #fff6e0, #ffd665)!important;
  background-image: -ms-linear-gradient(top, #fff6e0, #ffd665)!important;
  background-image: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #fff6e0), color-stop(100%, #ffd665))!important;
  background-image: -webkit-linear-gradient(top, #fff6e0, #ffd665)!important;
  background-image: -o-linear-gradient(top, #fff6e0, #ffd665)!important;
  background-image: linear-gradient(#fff6e0, #ffd665)!important;
  border-color: #ffd665 #ffd665 hsl(44, 100%, 64%)!important;
  color: #333 !important;
  text-shadow: 0 1px 1px rgba(255, 255, 255, 0.39)!important;
  -webkit-font-smoothing: antialiased!important;

}

.container{

width:100% !important;	
	
}

.menu{
position:fixed; left:0px; height:100%; z-index:10; white-space:nowrap; overflow-x:hidden; overflow-y:scroll; z-index:20; background-color:#f6e6d0;
	
}


.main{
	
position:absolute; right:0px;  min-height:100%; background-color:#e5d2ba;	z-index:19;
}

	 @media (max-width: 1139px) {

.menu.fadeout{
	
		 
	  -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
	width:0%;
	
}

.menu.fadein {
  -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
  width:300px;
  background-color:#FFF;
  
}



.main.menufadein{
	 -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
	width:100% ;
	
}

.main.menufadeout{
	 -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
	width:100%;
	
}


	 }
	 
	  @media (min-width: 1140px) {

.menu.fadeout{
	
		 
	  -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
	width:0%;
	
}

.menu.fadein {
  -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
  width:20%;
}



.main.menufadein{
	 -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
	width:80% ;
	
}

.main.menufadeout{
	 -webkit-transition: .2s;
  -moz-transition: .2s;
  -ms-transition: .2s;
  -o-transition: .2s;
  transition: .2s;
	width:100%;
	
}


	 }
	 
	 
	 
	 
</style>
<? if($url == "template"){ ?>
<link href="http://qeevee.org:9091/assets/css/bootstrap.css" rel="stylesheet">
      <link href="http://qeevee.org:9091/assets/css/bootstrap-responsive.css" rel="stylesheet">
<link href="http://quest-mill.com/portals/slsspiele/datatables.css" id="dtCss" rel="stylesheet">
<? } ?>

<link rel="icon" href="favicon.ico" type="image/x-icon" />
<base href="http://quest-mill.com/portals/slsspiele/index.php" />
</head>

<body link="#000000" vlink="#000000" alink="#000000" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
<div>




<img src="icons/menu.png" style="z-index:1000000; position:fixed; top: 10px; left:10px; cursor:pointer;" class="showmenu"  width="30"/>
<div style="position:fixed; top:0; left:0; width:100%; background-image:url('logo.png'); background-position:center; height:50px; z-index:100;">
<div style="position:absolute; top:0; left:0; width:100%; background-size:cover; background-image:url('banner.jpg'); background-position:center; background-repeat: no-repeat; height:50px; z-index:100;">

&nbsp;
</div></div>
<div style="position:fixed; top:0; left:0; width:100%; background-image:url('logo.png'); background-size:contain; background-position:center; height:50px; z-index:102; background-repeat:no-repeat;">
</div>

<div class="menu" >

<div id="menu_content" style="position:absolute; left:10px; top:55px; width:100%; padding-left:5%;  white-space:nowrap;">
<br/><br/>

  
  <ul style="list-style-type: none; padding:0; margin: 0 0 0 0;">
  
  <? if($url == "template"){ ?>
              <li>%%_GEOQUEST_NAV_LI_%%{
              %publicgameslink:=!!"hide"!!;;
              %newsstreamlink:=!!"hide"!!;;
			  %showlogout:=!!"true"!!;;
              }</li>
            <?  } ?>

  </ul>
 <br/>
 <br/>
 <hr/>
   <a href="http://wikiculturecity.com/impressum.html" target="_blank"> Impressum</a>

 
  
</p>
</div>

</div>

<div class="main">
<div id="tutorial_bump" style="height:55px; width:90%; margin-right:5%;">&nbsp;
</div>
<div id="tutorial_content" style=" width:90%; padding-left:5%;">
<? if($url == "template"){  ?>
	<div class="row-fluid">
      <h1>%%_GEOQUEST_CONTENT_TITLE_%%  </h1>
	</div>	

<div class="row-fluid">

%%_GEOQUEST_CONTENT_CODE_START_%%{
%general.language.Meine_Spiele:=!!"Meine Spielbeschreibungen"!!;;
%general.language.Newsstream:=!!"Neuigkeiten"!!;;
%general.language.Oeffentliche_Spiele:=!!"Alle Spielbeschreibungen"!!;;
%general.includesites.Newsstream:=!!"false"!!;;
%general.includesites.Oeffentliche_Spiele:=!!"true"!!;;
%general.language.Neues_Spiel_erstellen:=!!"Neue Spielbeschreibung anlegen"!!;;
%general.language.Spielname:=!!"Name der Spielbeschreibung"!!;;
%general.language.Spieltyp:=!!"Typ der Spielbeschreibung"!!;;
%general.language.hat_ein_neues_Spiel_erstellt:=!!"hat eine neue Spielbeschreibung angelegt"!!;;
%general.language.Du_hast_keine_Spiele:=!!"Du hast noch keine eigenen Spielbeschreibungen angelegt"!!;;
%general.authpage.style:=!!"posterview"!!;;
%general.games.directotoeditor:=!!"true"!!;;
%general.games.adminshaveallrights:=!!"true"!!;;
%general.games.adminsgetnotified:=!!"true"!!;;
%general.games.adminshavetopublish:=!!"true"!!;;
%general.button.attributes:=!!" class="btn btn-primary" "!!;; %general.speciallinks.attributes:=!!" class="btn btn-success" "!!;; %general.navtabs.attributes:=!!" class="btn btn-success" !!";; %general.tabs.type:=button-group% }
</div>
<? } ?>


</div>


</div>
</div>
<? if($url == "template"){ ?>
<script src="http://code.jquery.com/jquery-2.0.0.js"></script>


  <script src="http://qeevee.org:9091/assets/js/jquery.dataTables.js"></script>
	<script src="http://qeevee.org:9091/assets/js/DT_bootstrap.js"></script>
        <script src="http://qeevee.org:9091/assets/js/bootstrap.js"></script>
        <? } else { ?>
        <script src="https://code.jquery.com/jquery-2.0.0.js"></script>

<? } ?>
<script>

var boxOne = document.getElementsByClassName('menu')[0];
var boxTwo = document.getElementsByClassName('main')[0];






var geoquest_menu_in=getCookie("geoquest_menu_in") || 'true';



if ($(window).width() < 1139) {
 boxOne.classList.remove('fadein');
	    boxOne.classList.add('fadeout');
		 boxTwo.classList.remove('menufadein');
		 boxTwo.classList.add('menufadeout');
} else {
if(geoquest_menu_in == 'true'){
	   document.getElementsByClassName('showmenu')[0].src = 'icons/back.png';
    boxOne.classList.add('fadein');
	 boxTwo.classList.add('menufadein');
	 		 boxTwo.classList.remove('menufadeout');
			 localStorage['geoquest_menu_in'] = 'true';
} else if(geoquest_menu_in == 'false'){

 boxOne.classList.remove('fadein');
	    boxOne.classList.add('fadeout');
		 boxTwo.classList.remove('menufadein');
		 boxTwo.classList.add('menufadeout');
}

}


   document.getElementsByClassName('showmenu')[0].onclick = function() {
	   console.log(this.src);
  if(this.src.indexOf('icons/menu.png') > -1) 
  { 
    this.src = 'icons/back.png';
    boxOne.classList.add('fadein');
	 boxTwo.classList.add('menufadein');
	 		 boxTwo.classList.remove('menufadeout');
			 setCookie("geoquest_menu_in","true",365) ;

	 
  } else {
    this.src = 'icons/menu.png';
    boxOne.classList.remove('fadein');
	    boxOne.classList.add('fadeout');
		 boxTwo.classList.remove('menufadein');
		 boxTwo.classList.add('menufadeout');
			 setCookie("geoquest_menu_in","false",365) ;


    
  }  
}





function setCookie(c_name,value,exdays)
{
var exdate=new Date();
exdate.setDate(exdate.getDate() + exdays);
var c_value=escape(value) + ((exdays==null) ? "" : "; expires="+exdate.toUTCString());
document.cookie=c_name + "=" + c_value;
}




function getCookie(c_name)
{
var i,x,y,ARRcookies=document.cookie.split(";");
for (i=0;i<ARRcookies.length;i++)
{
  x=ARRcookies[i].substr(0,ARRcookies[i].indexOf("="));
  y=ARRcookies[i].substr(ARRcookies[i].indexOf("=")+1);
  x=x.replace(/^\s+|\s+$/g,"");
  if (x==c_name)
    {
    return unescape(y);
    }
  }
}


</script>

</body>
</html>
