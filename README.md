![](https://i1.visualstudiogallery.msdn.s-msft.com/2ab58875-5ead-44aa-b3e8-52965b8bd47d/image/file/137602/16/thumbnail.png) Config Transformation Visual Studio Extension
==================

Version 1.3.7.0

Author marazt

Copyright marazt

License The MIT License (MIT)

Last updated 09 May 2016

[Visual Studio Gallery Link](https://visualstudiogallery.msdn.microsoft.com/2ab58875-5ead-44aa-b3e8-52965b8bd47d)



About
-----------------

This extension enables ability to transform various config file even there does not exist build definition for the transformation file.  
For example, if you have multiple configuration transformations, e.g., *web.Debug.config*, *web.Release.config*, *web.UAT1.config*, *web.UAT2.config*, 
*web.UAT3.config*, *web.Production1.config*, *web.Production2.config*. 
And you want switch between these configuration while developing without changing actual build definition or editing the web.config manually. 



Abilities
-----------------
+ Generate configuration transformation while developing.
+ Preview transformation result.
+ Application output (transformation progress, errors, infomation) in **General Output window**.
+ Configuration in **Tools** -> **Options** -> **Config transformation menu**.
+ Transformation file nesting, can be set in **Tools** -> **Options** -> **Config transformation menu**.
+ Supports Visual Studio 12, Visual Studio 13, Visual Studio 15.



How To Use It
-----------------
+ File you want to change must be checked out (not locked, not readonly).
+ Click on the transformation configuration you want to run with the right mouse button and click "Execute transformation" option.

![](http://i1.visualstudiogallery.msdn.s-msft.com/2ab58875-5ead-44aa-b3e8-52965b8bd47d/image/file/137770/1/preview2.png)

+ Extension options can be found in **Tools** -> **Options** -> **Config Transformation**

![](https://i1.visualstudiogallery.msdn.s-msft.com/2ab58875-5ead-44aa-b3e8-52965b8bd47d/image/file/207805/1/options.png)

+ Information about transformation can be found in **General Output Window**.
+ You can see the transformation preview by **Preview transformation** menu item.



TODO
-----------------
* Unit tests - because the project was a long time private and written very fast for internal purposes, no tests were written, although they should :/.


Versions
-----------------


**Version 1.3.7.0 - 2016/04/09 (Added to GitHub)**

* New option item Write attributes on a separate line added. If this option is set to true, transformed config has every attribute on separate line. Default value is false.



**Version 1.3.6.0 - 2015/09/12**

* Removement of the .NET 4.5 dependency which caused that it was not possible to install the extension although the framework was correctly installed.



**Version 1.3.5.0 - 2015/09/12**

* Change of the temporary config file to ".config" instead of ".tmp" to enable syntax coloring.



**Version 1.3.4.0 - 2015/08/04**

* Added support for Viasu Studio 2015.



**Version 1.3.3.0 - 2014/09/23**

* Bug fix: Added information message about readonly file which should set to readwrite or checked-out.



**Version 1.3.2.0 - 2014/07/30**

* Added transformation files nesting option.



**Version 1.3.1.1 - 2014/07/20**

* Bug fix



**Version 1.3.1.0 - 2014/07/17**

* Added Preview transformation ability.



**Version 1.3.0.0 - 2014/07/16**

* Added logging to **General Output Window**.



**Version 1.2.0.0 - 2014/07/16**

* Added options menu to set extension configuration in **Tools** -> **Options** -> **Config Transformation**.