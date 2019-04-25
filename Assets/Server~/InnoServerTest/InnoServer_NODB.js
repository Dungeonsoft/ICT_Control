////////////////////////////////////////////
/// ------------------------------------ /// 
/// --이노시뮬레이션 서버 개발 테스트--- ///
/// ------------------------------------ /// 
////////////////////////////////////////////

// 기본 세팅 무조건 들어감.
// 모듈 추출이라도 함.
// C#의 using과 같은 역할이라고 보면 됨.
var app = require('express')();
var http = require('http').Server(app);
//var socketio = require('socket.io')(http);
var socketio = require('socket.io')(http);
// 기본 세팅 무조건 들어감.

// 몽고디비 설정을 위한 Require.
//var mongoClient = require('mongodb').MongoClient;
//var mongoURL = 'mongodb://localhost:27017/';

// DB 추가 관련 변수들.

var myobj;  // 추가할 데이터.
var mClient;
// DB 이름을 지정할 변수.
var nameDB = 'innodb';
// DB 안의 컬렉션(실제 데이터가 저장되는 엑셀문서 시트라고 생각하면 편함)
var nameClt = 'user_collection';
// 선택된 DB. DB와 연결된 뒤로는 이걸 기반으로 코드를 작성한다.
var dbo;
// 아이디 기억을 위한 변수.
var id = 0;

// 통제툴프로그램 아이디를 기억하기 위한 변수.
var idControlTool;

// 유저 이름을 숫자로 붙이기위해 필요한 변수.
// 유저가 들어올때마다 카운트를 올려준다.
var traineeNum = 0;
var roleNum = 0;

var uidList = new Array();
// 시작순서 0.
// 현재 Node.js 서버 스크립트를 실행하기 전에 먼저 몽고디비를 실행하여야 한다.
// 몽고디비 실행파일의 위치는 C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe

var sceneNum = 1;

// 시작순서 1.
// 프로그램을 시작하면 서버가 시작되고 클라이언트 접속을 받을 준비를 시킴.
http.listen(80, () => {
    console.log('*****************************************');
    console.log('*****************************************');
    console.log('**                                     **');
    console.log('** ClassMate Simulation Server On: 80  **');
    console.log('**                                     **');
    console.log('*****************************************');
    console.log('*****************************************');
});

// 시작순서 2.
// 몽고디비 연결 시작.(이미 몽고디비는 실행되어있음)
// 지정된 이름으로 (33번줄 NameDB 변수에 설정되어 있는 이름) 데이터베이스 생성.
//mongoClient.connect(mongoURL, (err, db) => {
//    if (err) throw err;
//    mClient = db;
//    dbo = mClient.db(nameDB);
//    dbo = db.db(nameDB);
//    console.log('DB Ready!');
//    dbo.createCollection(nameClt, (err,res) => {
//        if (err) throw err;
//        console.log('Collection: ' + nameClt+ ' ::: Created!!!');
//    });

//});

// 최초 클라이언트(개별)에서 접속이 시작될 경우 'connection'으로 연결이 되고.
// 커넥션 성공시 바로 아래에 있는 내용들이 실행되거나 혹은 실행(접속) 가능한 상태로.
// 유지가 된다.
socketio.on('connection', (socket) => {

    var date = new Date();
    console.log('Show Date: ' + date.getDate());
    console.log('Show day of the week: ' + date.getDay());

    id = socket.id;
    console.log('socket.id: ' + id);

    // 지정된 클라이언트(ID 기반)에 자동생성된 고유아이디를 알려준다.//
    // 이 방법은 소켓 통신을 private으로 하여 현상황에서 통신한 클라에게만//
    // 다시 필요한 정보(고유 ID)를 보내주는 방식이다.//

    var idAndDate = { 'id': id, 'date': date };
    socketio.sockets.to(id).emit('GetIdMade', idAndDate);
    socketio.sockets.to(id).emit('changeScenario', sceneNum+"");


    // 통제툴 로그인//
    // 아래 인자로 되어있는 clientMethod 이 것은 클라이언트 람다함수쪽//
    // 후 실행 함수 내용을 지칭하는 것이다. 이름에 대한 특별한 기준은 
    socket.on('loginControl', (_idControlTool, clientMethod) => {
        console.log("통제툴 아이디: " + _idControlTool);
        idControlTool = _idControlTool;
        clientMethod('Input success Control tool ID');

        uidList.length = 0;
        traineeNum = 0;
        roleNum = 0;
        console.log('서버 데이터 초기화');
    });

    // 클라이언트 로그인.
    // 클라이언트 로그인은 통제툴 로그인이 안되어 있으면.
    // 진행이 안되도록 설정을 한다.

    // 현재 여기서 데이터를 받았는데(_idClient) 읽히지 않는다.//
    // 이유를 모르겠다//
    // 2018.5.24 아침 8시//
    socket.on('loginClient', (_idClient, clientMethod) => {
        console.log("uidList1111::: " + _idClient);

        // 첫번째에 아직 통제툴이 로그인이 안되어 있는지 확인한다.//
        if (idControlTool === null || idControlTool === "") {
            // 안되어 있으면 false 를 반환하고.
            clientMethod('false');
        }
        else {
            // 되어 있으면 true를 반환한다.
            clientMethod('true');

            uidList.push(_idClient);

            console.log("uidList2222::: " + _idClient);
            // 그리고 아이디를 등록한다.
            // 먼저 몽고디비에 저장(등록)한다.
            // 클라이언트 아이디를 기반으로 우선 DB내의 컬렉션에 같은 것이 있는가를 찾는다.
            traineeNum++;



            ////////////////////////////////////////////////////////////
            // 입력된 숫자를 두자리를 가진 숫자(문자)로 변환하는 부분 //
            ////////////////////////////////////////////////////////////
            var number = traineeNum;
            var length = 2;

            number = number + "";//number를 문자열로 변환하는 작업
            var str = "";
            for (var i = 0; i < length - number.length; i++) {
                str = str + "0";
            }
            str = str + number;
            console.log(str);
            ////////////////////////////////////////////////////////////
            // 입력된 숫자를 두자리를 가진 숫자(문자)로 변환하는 부분 //
            ////////////////////////////////////////////////////////////


            var tName = 'Trainee' + str;
            roleNum++;
            if (roleNum> 2 && roleNum <= 5) roleNum = 5;
            if (roleNum > 5 && roleNum <= 10) roleNum = 10;
            if (roleNum > 10) roleNum = 0;
            myobj = {
                'uid': _idClient,
                'name': tName,
                'role': roleNum.toString(),
                'status': 'Ready',
                'result': 'Pass'
            };
            // 데이터베이스에 추가 데이터 입력.
            socketio.sockets.to(socket.id).emit('insertdata', 'End');
            //아래처럼 코드 작성해도 똑같은 기능이 작동함.
            //socketio.sockets.to(socket.id).emit('insertdata', 'End');

            // Close는 클라이언트를 완전히 끊을때만 쓰자.
            //mClient.close();

            // 이제 DB와 클라에 정보를 보내고 완료를 했으니.
            // 통제툴에 정보를 넘기는 것을 한다.
            console.log('트레이너 정보를 입력한다.::: ' + idControlTool);
            socketio.sockets.to(idControlTool).emit('createTraineeTab', myobj);
        }
    });

    // 클라이언트들을 멈춘다//
    socket.on('stopClients', () => {
        console.log('Server send stop sign to clients.');

        // 브로드 캐스팅 때에는 콜백을 할 수 없으니 함수만 호출한다.//
        for (var i in uidList) {
            console.log("uidList data:  " + uidList[i]);
            socketio.sockets.to(uidList[i]).emit('stopClient',"Change to Lobby Scene");
        }

        console.log('*****************************************');
        console.log('**           시험 강제 중지            **');
        console.log('**     모든 클라이언트 중지 완료       **');
        console.log('*****************************************');

    });

    // 클라이언트들을 시작한다//
    socket.on('startClents', (roleData) => {
        console.log('Server send start sign to clients.');

        // 브로드 캐스팅 때에는 콜백을 할 수 없으니 함수만 호출한다.//
        //socket.broadcast.emit('startClient');

        // uidList 길이를 알아본다.//
        console.log("uidList: " + uidList.length);

        for (var i in uidList) {
            console.log("uidList data:  " + uidList[i]);
            socketio.sockets.to(uidList[i]).emit('startClient', roleData);
        }

        console.log('*****************************************');
        console.log('**              시험 시작              **');
        console.log('**     모든 클라이언트 시험 시작       **');
        console.log('*****************************************');

    });

    socket.on('changeScenario', (sn) => {

        sceneNum = sn;
        console.log('시나리오 변경 :: '+ sceneNum);

        // for (var i in uidList) {
        //     console.log("uidList data:  " + uidList[i] +"변경된 에피소드에 맞춰 콘텐츠 데이터 전달");
        //     socketio.sockets.to(uidList[i]).emit('changeScenario', sceneNum);
        // }
        console.log('*****************************************');
        console.log('**           시나리오 변경             **');
        console.log('**       콘텐츠 정보 전달 완료         **');
        console.log('*****************************************');
    });


    //3년차에서 컨트롤툴과 클라이언트와 연계하여 완료하여야 할 부분.
    socket.on('changeMode', (modeNum) => {
        console.log('모드 변경 :: ' + modeNum);

        // 시간에 대한 상태를 변경할 때 정보를 Control에서 받아와서.
        // 각 클라이언트에 보내주는 역할을 한다.
        for (var i in uidList) {
            if (uidList[i] !== userData.uid) {
                console.log('changeMode:: ' + modeNum);
                socketio.sockets.to(uidList[i]).emit('changeMode', modeNum);
            }
        }
        console.log('*****************************************');
        console.log('**             모드 변경               **');
        console.log('**           모드 변경 완료            **');
        console.log('*****************************************');
    });
    

    //유저의 역할을 변경하였을 경우 그 정보를 각 클라이언트에 전달.
    socket.on('sendChangedUserData', (uDataJson) => {

        console.log('uDataJson ::: ' + uDataJson.uid);
        console.log("Role:: " + uDataJson.role);
        socketio.sockets.to(uDataJson.uid).emit('changeRole', uDataJson.role);
        console.log(":::Role 정보 전달 완료:::");
    });

    //유저의 위치 회전 정보를 중계해주기 위한 부분.
    socket.on('userPozRot', (userData) => {

        //console.log("UserData ID: " + userData.uid);
        //console.log("UserData Action Number: " + userData.actionNum);
        //console.log("UserData Role Number: " + userData.roleNum);

         for (var i in uidList) {
            if (uidList[i] !== userData.uid) {
                //console.log('userData:: ' + userData);
                // socketio.sockets.to(userData.uid).emit('getOtherPoz', userData);
                socketio.sockets.to(uidList[i]).emit('getOtherPoz', userData);
            }
        }
    });

    //유저가 모든 시험을 마치고 종료시 이곳으로 완료정보를 보낸다.
    //보낸 정보는 다시 컨트롤 툴로 전달하여 화면에 완료정보가 보이도록 한다.
    socket.on("alarmclear", (userData) => {
        console.log("UserData ID: " + userData.uid);
        console.log("UserData Role Number: " + userData.roleNum);
        console.log("UserData Action Clear: " + userData.actionClear);

        //컨트롤 툴에 완료 정보가 보이도록 한다.
        socketio.sockets.to(idControlTool).emit('alarmclear', userData);
    }
    );

    // 유저의 연결을 끊고 컨트롤 툴의 정보(서버의 현재 정보)를 초기화 하기 위한 부분. 
    socket.on('resetUserList', () => {
        console.log('User List Lengh111: ' + uidList.length);

        for (var i in uidList) {
            console.log("Delete User ::: " + uidList[i]);
            socketio.sockets.to(uidList[i]).emit('quitApp','quit');
        }
        traineeNum = 0;
        sceneNum = 1;
        uidList.length = 0;
        traineeNum = 0;
        roleNum = 0;
        console.log('User List Lengh222: ' + uidList.length);
    });

    socket.on('sendChangeWeather', (userData) => {
        console.log("날씨 변경");

        for (var i in uidList) {
            console.log('날씨:: ' + userData);
            socketio.sockets.to(uidList[i]).emit('getChangeWeather', userData);
        }
    });
    
});

//컨트롤툴에서 데이터를 보내고 이것을 서버코드에서 받아서 각 클라이언트에 뿌리는 것까지 완료.
//이제 클라이언트에서 데이터를 받는 부분을 작성.
//컨트롤툴에서 날씨를 선택하면 그 데이터가 저장되는 부분도 필요.
//클라이언트(클라의 ClientLobbyServerConnect.cs)에서는 우선 받아서 constaData부분에 임시로 날씨정보를 저장한다음.
//이 것을 실제 씬이 실행될 때 씬에서 받아와서 안개가 낀것처럼 만들어주면 된다.
//현재 클라로 데이터를 넘겨서 ConstDataScript.cs 파일의 string weatherTyp 에 저장되도록 구성이 되었다.

//Role
//0: 랜덤 셀렉트//
//1: 선장//
//2: 1항사//
//3: 2항사//
//4: 3항사//
//5: 기관장//
//6: 1기사//
//7: 2기사//
//8: 3기사//
//9: 갑판장//
//10: 갑판수A//
//11: 갑판수B//
//12: 갑판수C//
//13: 조기장//
//14: 조기수A//
//15: 조기수B//
//16: 조기수C//
//17: 조리장//
