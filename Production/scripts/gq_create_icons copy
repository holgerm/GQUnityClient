#!/bin/bash

echo "Welcome to create_app_icons!"

if [ ! "$#" -eq 2 ]; then
	echo "  usage: createAppIcons <sourceIconPath> <target_dir_path_absolute>"
	exit 1
fi

if [ ! -f "$1" ]; then
	echo "  first argument (sourceIconPath) does not seem to be an existing readable file."
	exit 1
else
	if [[ ! $1 =~ \.(png|jpg)$ ]]; then
		echo "  first argument must be either a png or jpg image file."
	fi
fi 

if [ ! -d "$2" ]; then
	echo "  creating target dir ..."
	mkdir -p $2
else
	if [ "$(ls -A $2)" ]; then
     	echo "  WARNING: target dir is not empty. We are overwriting. Please check output carefully."
     	# exit 1
	fi
fi 



sizes="192 180 152 144 120 114 96 76 72 57 48 36"

for size in $sizes
	do
		echo "  creating $2/appIcon_${size}.png"
		convert $1 -resize ${size}x${size} $2/appIcon_${size}.png	
	done

