#Modelviewer for Monogame

##Features:

- Import a variety of .obj and .fbx files. They must contain texture coordinates. A mesh cannot have different materials for submeshes (yet?).#
- Meshes without normals are supported. Meshes without texcoords/uv are supported, too, but they won't display textures.
- Skinned Meshes with up to 4 bones per vertex are supported
- Animated Skinned Meshes
- plausible environment lighting, with support for 4 texture inputs - albedo, normal, roughness, metallic (no alpha yet)
- HBAO solution
- Parallax Occlusion Mapping
- Intuitive UI

##Controls

- LMB drag the object
- RMB -rotate around the object
- Shift+RMB - rotate the environment
- MW - zoom in/out
- space - center object



##Version history:

0.6 - Added Parallax Occlusion mapping to textures. Added the ability to shift-right click to rotate the sky. Added button to center model.
0.7 - added mip mapping again


Cubemaps from
http://www.humus.name/index.php?page=Textures

by Emil Persson, licensed under Creative Commons Attribution 3.0 Unported License.

