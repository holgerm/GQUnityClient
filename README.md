# GQUnityClient
Our shiny new Unity based Client


## Git Tricks ##

### Marking Files Assumed Unchanged ###

Product spec files do change frequently and should not be updated in the git. But there should be at least some version of each file, to keep the product editor and Unity IDE working correctly. Examples are Assets/ConfigAssets/Resources/appIcon.png as placeholder for the app icon etc. 

We checked these files into git and afterwards marked them so that git ignores further changes. Here is how we do that:

Use this:

	git update-index --assume-unchanged Assets/ConfigAssets/Resources/*

And to restore:

	git update-index --no-assume-unchanged path/file.cfg

Lastly, if you want to list files that are marked with assume-unchanged:

	git ls-files -v | grep ^h | awk '{print $2}'

To simplify, you can make an alias for that in your $HOME/.gitconfig:

	[alias]
    	  ls-assume-unchanged = !git ls-files -v | grep ^h | awk '{print $2}'

Then you can type just git ls-assume-unchanged. It even works with auto-completion if you have the git-completion in place (for bash, tcsh, zsh).


Using SmartGit one can set files to assume-unchanged state in the Local menu using "toggle assume-unchanged", and one can filter these files in or out in the Files view.