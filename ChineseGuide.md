# Introduction #

本软件需要Microsoft .Net Frameworks 2.0的支持。如果您的机器中没有安装Microsoft .Net Frameworks 2.0，请使用下面的网址进行安装：
> http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5&DisplayLang=zh-cn

> 提示：Vista系统已经内置Microsoft .Net Frameworks 2.0。

本软件支持RAR、ZIP、7z压缩文件。

_如果需要自己维护data,请下载源代码，然后以Debug模式编译即可有data编辑选项。_



# 下载/安装/升级 #

## 下载 ##
从下载栏目中下载最新版的软件

![http://swift-rom.googlecode.com/svn/wiki/ch-download.png](http://swift-rom.googlecode.com/svn/wiki/ch-download.png)

## 安装 ##
把下载的文件解压至任意目录,双击Swift.ROM.exe文件进入本软件。

![http://swift-rom.googlecode.com/svn/wiki/ch-file.png](http://swift-rom.googlecode.com/svn/wiki/ch-file.png)

## 升级 ##
下载新版软件，直接覆盖旧文件(目录)即可。

# 主界面 #

![http://swift-rom.googlecode.com/svn/wiki/ch-main.png](http://swift-rom.googlecode.com/svn/wiki/ch-main.png)

界面上面为菜单和工具栏.

左半部分为游戏列表,其中浏览NDS游戏列表时，系统会自动显示游戏的图标。

右半部分是显示游戏的截图、附加信息以及包装等图片的。

下部用于显示当前选中游戏对应的文件，已经升级、验证、ROM收集数量等信息。

# 更改列表 #
v9.05.01
  * 修复"另存为..."功能中读取文件名称的错误.
  * 更新NDS ROM信息.

v8.11.19
  * 更新NDS ROM信息。

v8.10.12
  * 修正错误Issue 2。
  * 修正错误Issue 3。
  * 更新NDS ROM信息。
  * 特别感谢 lxbzmy、willypoison 提交的BUG信息和建议。

v8.09.29
  * 增加CRC识别ROM的方式。这样可以直接读取压缩文件中的ROM CRC32值用于识别，提高了识别速度。
  * 更新NDS ROM信息。
  * 特别感谢Lobster DB督促我实现CRC识别:)

v8.09.11
  * 增加ROM相对路径存储。如果把游戏ROM放在本软件的目录下，则将在已识别ROM里面使用相对路径存储，方便移动硬盘用户使用。感谢sf.net中匿名提出建议的朋友。
  * 更新NDS ROM信息。

v8.08.24
  * 更新NDS ROM信息。

V8.08.08
  * 删除一个关于sf.net的内容。
  * 更新NDS ROM信息。

v8.08.06
  * 优化显示图标时的代码。
  * 更新NES(FC) ROM信息。
  * 更新NDS ROM信息。

v8.07.27
  * 增加全部NDS ROM图标。
  * 更新NDS ROM信息。

V8.07.18
  * 增加主界面右边的信息栏显示的显示信息。
  * 增加游戏图标显示功能。
  * 为方便游戏图鉴的上传和管理，重新安排游戏图鉴的目录结构,把以前各机种放在一个单一目录中改为多层目录。
  * 修正"视图"菜单中"列表"和"细节"无选中状态的错误。
  * "视图"菜单中增加"平铺"显示方式。
  * 升级文件存储格式变动，更改后的升级文件要比以前的升级文件更小。
  * 更新部分关于sf.net转到googlecode.com关联的代码。
  * 更新NDS ROM信息。

V8.07.13
  * 由于sf.net网站无法访问,本项目转到 http://swift-rom.googlecode.com
  * 更新NDS ROM信息。

V8.06.19
  * 更新NDS ROM信息。

V8.06.04
  * 修正提示窗口在16位色显示模式下产生镂空的错误。
  * 更新NDS ROM信息。
  * 更新NES(FC) ROM信息。

V8.05.25
  * 新增识别iDS游戏ROM。
  * 更新NDS ROM信息。

V8.05.17
  * 减少提示窗口延时显示时间。
  * 改进提示窗口中的内容显示方式。
  * 更新NDS ROM信息。
  * 更新NES ROM信息。
  * 感谢在我的Blog中留言的朋友。

V8.05.08
  * 本版为奥运珠峰特别版。
  * 在"设置"中,可以设置自动下载图片时是否下载扩展图片（例如：封面图片、卡带图片等）。如果设置为不下载扩展图片时，可以在游戏列表上使用右键单独下载指定游戏的扩展图片。
  * 启用本软件Wiki,http://swift-rom.wiki.sourceforge.net ,以后使用说明等信息我会逐步的放在这里面。
  * 更新NDS ROM信息。
  * 感谢 bbs.emu-zone.org 中 模型天使 提出的修改建议。
  * 求助：大家看看 http://sourceforge.net/projects/swift-rom 网站谁不能访问，请告知使用网络情况，例如：北京网通ADSL等。

V8.05.05
  * 更新随带软件DeSmuME 0.8。
  * 更新NDS ROM信息。

V8.04.20
  * 信息栏显示从4行扩展为6行。
  * 更新NES(FC) ROM信息。
  * 更新GBA ROM信息。
  * 更新NDS ROM信息。

V8.04.13
  * 更新/整理NDS ROM信息。
  * 整理GBA ROM信息。
  * 感谢bbs.emu-zone.org中wwwzhuobin提出的增加汉化ROM的建议。此次更新就是更新了一批汉化ROM,但是现在汉化ROM太多、太乱，实在是很难收集全，如果大家谁手里有本软件未收录的软件，请把ROM或者ROM下载地址发给我，谢谢：） 请发到 0123@163.com

V8.04.02
  * 修正几个没有翻译的菜单。
  * 更新NDS ROM信息。
  * 更新NES ROM信息。
  * 整理GBA ROM信息。
  * 感谢www.emu-zone.org网站对本软件的支持。

V8.03.23
  * 增加“另存为”可选择是游戏文件还是压缩文件。
  * 更新NDS ROM信息。
  * 更新NES ROM信息。
  * 感谢在sf.net论坛中匿名提出建议的朋友。

V8.03.11
  * 在设置窗口增加关闭本软件自动升级检测的选项。
  * 更新NDS ROM信息。

V8.02.22
  * 增加ROM列表打印功能。
  * 加快ROM列表刷新速度。
  * 更新NDS ROM信息。
  * 发布2008年01月更新的游戏图片压缩包。
  * 开发平台升级到VS2008。

V8.01.28
  * 再次修正仅升级数据文件的代码，为将来自动在线升级做好准备。
  * 更新NDS ROM信息。
  * 欢迎 WangBin 加入到本项目中。

V8.01.23
  * 修正仅升级数据文件的代码，为将来自动在线升级做好准备。
  * 更新NDS ROM信息。

V8.01.13
  * 更新NDS ROM信息。

V8.01.08
  * 增加对多模拟器的支持。多种模拟器在设置中添加，打开游戏时双击为默认模拟器，点右键可选择其他模拟器。
  * 修正一个识别压缩文件时的错误。
  * 更新NDS ROM信息。

V7.12.23
  * 修正一个翻译问题。
  * 更新NES/GBA/NDS ROM信息。

V7.12.15
  * 修正当ROM文件名称不改变、内容改变时不自动重新验证ROM的错误。新的验证方式是在ROM数据文件中增加一个文件的最后修改时间的值，在验证ROM时会自动比较这个值，如果有改变则认为ROM文件已经被改变，需要重新识别。
  * 更新NES/GBA/NDS ROM信息。

V7.12.08
  * 修复验证压缩ROM时对无需解压的文件的处理。以前验证压缩ROM时，所有的压缩文件都先进行解压，再进行验证，但是很多的情况下会出现压缩文件中不存在需要验证的ROM。此次修改后，解压之前会先判断一下解压文件中是否存在需要验证的文件，然后再判断是否需要解压。
  * 更新NES/GBA/NDS ROM信息。

V7.12.01
  * 修复下载图片时可能出现图片重复下载的错误。
  * 更新NES/GBA/NDS ROM信息。
  * 图片打包下载的问题，我打算等到2008年后我把所有的2007年的图片打成一个包供下载。2008年以后每次发布新版都跟一个图片的更新包。
  * 感谢 沙鸥 提供no$gba修改ROM的信息。
  * 感谢 tobyliu415 的支持。

V7.11.25
  * 在使用ROM编号排序显示时，会自动增加游戏名称的二级排序。
  * 修正设置窗口中部分翻译问题。
  * 更新NES/GBA/NDS ROM信息。
  * 删除原来在sourceforge.net服务器上的图片,以后只能使用新版的软件下载游戏图片。

V7.11.18
  * 修复一个识别压缩文件的错误。在识别文件的时候，如果文件中存在"["或"_"字符会发生文件不能够识别的情况，现已修复。
  * 更改中文版的升级提示为中文。
  * 更新NES/GBA/NDS ROM信息。_

V7.11.12
  * 增加仅更新数据文件的升级格式。这样以后更新的时候，如果程序没有改动的话将直接发升级数据包。为将来自动在线升级做好准备。
  * 更新NES/GBA/NDS ROM信息。

V7.11.4
  * 更换图片下载服务器。现在的服务器不如以前的稳定，但是实在也没有别的好办法了，将就着用吧。
  * 更新NES/GBA/NDS ROM信息。

V7.10.21
  * 更新NES/GBA/NDS ROM信息。

V7.10.14
  * 增加屏蔽显示非重要ROM的选项。
  * 当软件不需要升级时，不显示不需升级的提示文本。
  * 首次使用本软件时，默认显示NDS信息。以前默认显示的是GBA信息。
  * 更改编辑ROM信息时图片编号的算法。修改后会根据情况的不同自动进行调整。
  * 更改编辑ROM信息时中文名称的转换，增加中文“：”符号自动转换为":"。
  * 更新NES/GBA/NDS ROM信息。

V7.10.7
  * 增加在ROM编辑时可以删除ROM图片功能。
  * 更改在ROM编辑时读取ROM编号的部分。修改后对无ROM编号的文件名不自动提取ROM编号。
  * 更新NES/GBA/NDS ROM信息。

V7.9.30
  * 修整一个翻译错误。
  * 修正显示NES图片时的排列方式。
  * 调整ROM信息存储格式,减少存储数据。理论上会提高处理速度。
  * 美化设置窗口。
  * 增加对ROM的等级管理，ROM可分为：“重要”、“非重要”、“可舍弃”三类，并以不同的颜色显示。“重要”主要指Clean ROM,“非重要”主要指被Crack ROM,“可舍弃”指那些有新版本的ROM。如果不想使用分级显示功能，可以到“设置”功能中关闭此功能。
  * 改变ROM信息编辑后的列表刷新方式，现在的方式速度更快。
  * 放入收藏夹中的ROM的图标改成收藏夹的图标。
  * 修改进度条只有在使用的时候才显示。
  * 更新NES/GBA/NDS ROM信息。
  * 修复错误#1802191。
  * 非常感谢gstanczyk的错误报告。

V7.9.23
  * 在验证ROM时，自动忽略带有密码的RAR文件。以前版本在遇到带有密码的RAR文件时会停止后面文件的识别。
  * 项目源代码中删除安装程序部分。
  * 删除ROM信息后台管理中增加图片的功能。因为这个功能早已合并到ROM信息编辑窗口里面。
  * 7z/Zip解压程序升级。主要提高了7z/Zip文件的解压速度和部分BUG的修复。
  * 更新NES/GBA/NDS ROM信息

v7.9.16
  * 发行版本去掉MSI格式，全部以压缩文件方式发布。
  * 增加自动检测软件升级功能。在有新版软件发布后，会自动进行提示并提示下载。
  * 启动时显示sourceforge.net的图标
  * 更新NDS ROM信息

v7.9.9
  * 增加游戏图片另存功能。
  * 修复一个识别压缩ROM的错误。如果很多压缩文件中有一个有毛病的压缩文件，程序会在遇到有毛病的压缩文件后停止其他ROM的识别，现在这个错误已经修复。
  * 修复“未识别的ROM”和“未知ROM”显示的错误。
  * 增加NES机型的支持。
  * 更新GBA/NDS ROM信息

v7.7.15
  * 修复一个保存文件的BUG。故障：会在软件目录保存一个ROM.xml文件，其实这个文件应该是保存在相应的机型目录里面的，这个BUG不会导致任何使用故障，现在这个BUG已经修复。在升级的时候可以放心的把软件目录下的ROM.xml文件删除。
  * 修复压缩文件名称带有单引号时无法识别ROM的错误。
  * 修复如果压缩文件中存在目录时无法识别ROM的错误。
  * 修复一个自动下载图片中的错误。如果在下载图片时出错（例如服务器不存在要下载的文件）就会生成一个0字节的文件，当软件需要这个文件的时候就会把这个0字节的文件作为一个正常文件来处理。修复后，当下载的时候识别到下载的文件是0字节时就自动把这个文件给删除掉。
  * 在非中文系统中，默认显示的中文名称列的宽度为0。
  * 升级附带NDS模拟器到DeSmuME v0.7.2。
  * 更新GBA/NDS ROM信息。

v7.7.9
  * Fixed Bug:1750093# v7.7.7存在一个致命的错误,这个错误会错误的重写 GBA\ROM.xml 文件!这个错误会导致本软件GBA ROM信息的丢失!如果你需要从v7.7.7升级到v7.7.9,请在升级前把 GBA\ROM.xml 和 Swift.ROM.xml 删掉。
  * 支持 7z 压缩格式。
  * 增加NDS ROM信息.
  * 感谢 ayzmsck 提供翻译支持。

v7.7.7
  * 支持 ZIP/RAR 压缩格式.
  * 在后台进行验证、识别ROM的工作.