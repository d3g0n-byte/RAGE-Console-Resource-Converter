# RAGE Console Resource Converter

### Description
<ul>
  <li>Tool to convert some resources <em>(like models, textures, etc.)</em> from console games on RAGE engine.</li>
</ul>

### Supported games
<ul>
  <li><a href="https://en.wikipedia.org/wiki/Red_Dead_Redemption" target="_blank">Red Dead Redemption</a></li>
  <li><a href="https://en.wikipedia.org/wiki/Midnight_Club:_Los_Angeles" target="_blank">Midnight Club: Los Angeles</a></li>
</ul>

### Features
<ul>
  <li>Convert models <em>(with animations)</em> to OpenIV's openFormats' files which can imported through <a href="https://gtaforums.com/topic/560813-3dsmaxrel-openiv-openformats-io/" target="_blank">openFormats I/O</a></li>
  <li>Convert textures to DDS format</li>
  <li>Unpack the resource file to memory segments <em>(for experts)</em></li>
  <li>Supported formats are:
    <ul>
      <li>.xfd/.xwt (v1)</li>
      <li>.xas (v6)</li>
      <li>.xtd (v9/v10)</li>
      <li>.xrsc (v63)</li>
      <li>.xtp/.xtl (v83)</li>
      <li>.xdr/.xdd (v102/v109)</li>
      <li>.xck (v131)</li>
      <li>.xvd (v133)</li>
      <li>.xsi (v134)</li>
      <li>.xft (v138)</li>
      <li>some others if the resource version and file contents are supported</li>
    </ul>
  </li>
</ul>

### Download
<ul>
  <li>All builds are available in <a href="https://github.com/d3g0n-byte/RAGE-Console-Resource-Converter/releases">Releases</a> page.
</ul>

### Usage
<ul>
  <li>
    Simple conversion example:<br />
    <pre>Converter.CLI.exe -i &lt;input_path&gt;</pre>
  </li>
  <li>
    Available options <em>(you can also run Converter.CLI.exe without arguments to see them)</em>:<br />
    <pre>-l, --basic-log
&nbsp;&nbsp;&nbsp;&nbsp;Basic logging (verbose) mode. Contains a basic details about opened resource.<br />
-e, --extended-log
&nbsp;&nbsp;&nbsp;&nbsp;Extended logging (very verbose) mode. Contains a basic + exporting details.<br />
-n, --nowait
&nbsp;&nbsp;&nbsp;&nbsp;Disable waiting for user input when all tasks are finished/failed.<br />
-i, --input
&nbsp;&nbsp;&nbsp;&nbsp;Required. The path to folder or file which needs to be converted.<br />
-o, --output
&nbsp;&nbsp;&nbsp;&nbsp;The path to folder(!) where you want to save the result.<br >&nbsp;&nbsp;&nbsp;&nbsp;If it's not specified, then output directory will be the same as input.</pre>
  </li>
</ul>

### Limitations
<ul>
  <li>Currently supported platform is Xbox 360 only. PlayStation 3 is not fully supported <em>(only unpacking to memory segments)</em>.</li>
  <li>At this moment, conversion results can be exported only to OpenIV's openFormats files. In future versions it may be changed.</li>
  <li>At this moment, only reading and converting the resources is possible. It means that you can't, for example, make some kind of modifications for that games.</li>
  <li>Only command-line (CLI) version of the converter is available. Graphical (GUI) version may be added in future versions.</li>
  <li>This tool can be run only on Windows. Linux/macOS/other OS are not supported officially, but if you can run that tool on it - it's great.</li>
</ul>

### Dependencies
<ul>
  <li><a href="https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472" target="_blank">.NET Framework 4.7.2</a> or newer.</li>
</ul>

### FAQ
<ul>
  <li>
    <strong>Q:</strong> How can I run your tool? When I double-clicking on EXE, it just displays me a help text.<br />
    <strong>A:</strong> Right-mouse-click on a empty space of Explorer window with folder where Converter.CLI.exe is located and choose "Open in Terminal", or, if you're on Windows 10, press Shift and then Right-mouse-click and choose "Open command prompt here", or simply click on address bar at the top of the Explorer window and type "cmd", then press Enter.<br />
    <strong><em>Sadly, many people still don't know this.</em></strong>
  </li>
  <li>&nbsp;</li>
  <li>
    <strong>Q:</strong> Can I convert &lt;my-file&gt;?<br />
    <strong>A:</strong> Read <a href="#features">Features</a> section, it has a list of supported formats.
  </li>
  <li>&nbsp;</li>
  <li>
    <strong>Q:</strong> How can I know the resource version of &lt;my-file&gt;?<br />
    <strong>A:</strong> Some information is explained <a href="https://web.archive.org/web/20200225230231/https://gtamodding.ru/wiki/RSC">here</a>. Use it and any hex editor you like to grab the information about resource version you want to know.
  </li>
  <li>&nbsp;</li>
  <li>
    <strong>Q:</strong> Can you add a &lt;feature-name&gt;?<br />
    <strong>A:</strong> Welcome to <a href="https://github.com/d3g0n-byte/RAGE-Console-Resource-Converter/issues">Issues</a> page. Just open a new one, and describe what exactly you want.
  </li>
</ul>

### Thanks
<ul>
  <li><a href="https://github.com/oxmaulmike2581" target="_blank">oxmaulmike2581</a> &ndash; for his great contribution to the development of this project and assistance.</li>
  <li><a href="https://github.com/Foxxyyy" target="_blank">Foxxyyy</a> &ndash; for his MagicRDR project</li>
  <li><a href="https://github.com/XBLToothPik" target="_blank">XBLToothPik</a> &ndash; for his AreDeAre xPlorer 2 project</li>
  <li><a href="https://web.archive.org/web/20140501000000*/dageron.com" target="_blank">Dageron</a> &ndash; for all his work to researching RAGE game engine</li>
</ul>

### Licensing
<ul>
  <li>This project is licensed under <a href="https://opensource.org/license/mit/" target="_blank">MIT License</a>, but some libraries used in this project may have their own licenses.</li>
</ul>
