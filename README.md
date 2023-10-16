# icebreaker

This is the first ever tool for editing cutscenes in NFS Most Wanted and limited viewing of them in some other NFS games. It targets EAGLAnim chunks in NIS bundles and CameraTrack chunks in InGameB bundle.

The code is horrendous and no good and very ugly. It came to be through rapid prototyping and black-box testing and then was never refactored. Still I believe that it being open source could be of some benefit to the community, since I don't plan to support it anymore.

You will need Unity 2021.1.24f1 to open this project. You can try other versions but it might break.

It uses <a href=https://github.com/NFSTools/NFS-ModTools>NFS-ModTools</a> to load preview geometry.

<b>Cool fact:</b> "NIS" files in NFS contain the same data format as "O" files in FIFA games by EA Vancouver. Because of this, tools for editing O files that allow to replace football players do the same thing as Icebreaker, except they cannot edit animations in them, while Icebreaker allows to edit animations only. One example of such "sister tool" is OEdit by Arushan (who was also early NFS modder!). There's actually a full <a href=https://fifam.miraheze.org/wiki/O>wiki page</a> about this file format by Dmitri, who made newer alternative to OEdit called OTools. Even though it's not intended for NFS at all, it provided a lot of information on how to parse these files. It's very interesting to see same things from different perspective.
