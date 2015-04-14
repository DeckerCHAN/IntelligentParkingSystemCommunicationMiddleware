#更新系统
###1.如何使用
将新版本的二进制文件统一打包（或者将需要更新的文件打包）为标准无密码的zip文件，在服务器端更新版本号后即可由客户端更新。
###2.使用过程
用户在点击更新后，系统会向服务器请求版本信息，当发现服务器提供的版本与当前版本不符后便会提示用户有新版本。用户点击确定后将会主程序启动Updater。Updater是独立于主程序的单一更新器，通过给更新器传递的参数，更新器将会下载特定的版本到update文件夹，并将启动的内容解压到temp文件夹。接下来，当更新器检测到主程序依然运行时，更新器将会等待主程序退出后再继续更新进程。
###3.注意事项
在打包更新包过程中，请不要将更新器内容打包进入。因为更新器下载更新包后无法替换自身。因此请将更新器编译后手动拷贝到发行版本中。