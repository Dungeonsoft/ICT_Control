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
var mongoClient = require('mongodb').MongoClient;
var mongoURL = 'mongodb://localhost:27017/';

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

// 시작순서 1.
// 프로그램을 시작하면 서버가 시작되고 클라이언트 접속을 받을 준비를 시킴.
http.listen(3000, () => {
    console.log('*************************************');
    console.log('*************************************');
    console.log('**                                 **');
    console.log('** INNO Simulation Server On: 3000 **');
    console.log('**                                 **');
    console.log('*************************************');
    console.log('*************************************');
});

// 시작순서 2.
// 몽고디비 연결 시작.(이미 몽고디비는 실행되어있음)
// 지정된 이름으로 (33번줄 NameDB 변수에 설정되어 있는 이름) 데이터베이스 생성.
mongoClient.connect(mongoURL, (err, db) => {
    if (err) throw err;
    mClient = db;
    dbo = mClient.db(nameDB);
    dbo = db.db(nameDB);
    console.log('DB Ready!');
    dbo.createCollection(nameClt, (err,res) => {
        if (err) throw err;
        console.log('Collection: ' + nameClt+ ' ::: Created!!!');
    });

});

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


    // 통제툴 로그인//
    // 아래 인자로 되어있는 clientMethod 이 것은 클라이언트 람다함수쪽//
    // 후 실행 함수 내용을 지칭하는 것이다. 이름에 대한 특별한 기준은 
    socket.on('loginControl', (_idControlTool, clientMethod) => {
        console.log("통제툴 아이디: " + _idControlTool);
        idControlTool = _idControlTool;
        clientMethod('통제툴 아이디 입력 완료');
    });

    // 클라이언트 로그인.
    // 클라이언트 로그인은 통제툴 로그인이 안되어 있으면.
    // 진행이 안되도록 설정을 한다.

    // 현재 여기서 데이터를 받았는데(_idClient) 읽히지 않는다.//
    // 이유를 모르겠다//
    // 2018.5.24 아침 8시//
    socket.on('loginClient', ( _idClient, clientMethod) => {
        console.log("uidList1111::: " + _idClient);

        // 첫번째에 아직 통제툴이 로그인이 안되어 있는지 확인한다.//
        if (idControlTool == null || idControlTool =="") {
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
            dbo.collection(nameClt).findOne({ _idClient }, (err, res) => {
                console.log("Response: " + res);

                traineeNum++;
                var tName = 'trainee' + traineeNum;
                roleNum++;
                if (roleNum > 17) {
                    roleNum = 1;
                }
                myobj = {
                    'uid': _idClient,
                    'name': tName,
                    'role': roleNum.toString(),
                    'status': 'Ready',
                    'result': 'Pass'
                };
                if (res == null) {
                    // 데이터베이스에 추가 데이터 입력.
                    dbo.collection(nameClt).insert(myobj, (err, res) => {
                        if (err) throw err;

                        socketio.sockets.sockets[socket.id].emit('insertdata', 'End');
                        //아래처럼 코드 작성해도 똑같은 기능이 작동함.
                        //socketio.sockets.to(socket.id).emit('insertdata', 'End');

                        // Close는 클라이언트를 완전히 끊을때만 쓰자.
                        //mClient.close();

                        // 이제 DB와 클라에 정보를 보내고 완료를 했으니.
                        // 통제툴에 정보를 넘기는 것을 한다.
                        console.log('트레이너 정보를 입력한다.::: ' + idControlTool);
                        socketio.sockets.to(idControlTool).emit('createTraineeTab', myobj);
                    });
                }
                else {
                    console.log('Use Other Uid');
                }
            });

        }
    });

    // 클라이언트들을 멈춘다//
    socket.on('stopClients', () => {
        console.log('Server send stop sign to clients.');

        // 브로드 캐스팅 때에는 콜백을 할 수 없으니 함수만 호출한다.//
        socket.broadcast.emit('stopClient');
    });

    // 클라이언트들을 시작한다//
    socket.on('startClents', () => {
        console.log('Server send start sign to clients.');

        // 브로드 캐스팅 때에는 콜백을 할 수 없으니 함수만 호출한다.//
        //socket.broadcast.emit('startClient');

        // uidList 길이를 알아본다.//
        console.log("uidList: " + uidList.length);
        // 아이디별로 개별 호출한다//
        //uidList.forEach(() => {
        //    console.log("User ID: " + value);
        //    socketio.sockets.sockets[id].emit('startClient', 'Start Client!!!!');
        //});

        for (var i in uidList) {
            console.log("uidList data:  " + uidList[i]);
            socketio.sockets.sockets[id].emit('startClient', 'Start Client!!!!');
        }
    });

    socket.on('sendChangedUserData', (uDataJson) => {

        console.log('uDataJson ::: ' + uDataJson.uid);
        console.log("Role:: " + uDataJson.role);
        socketio.sockets.to(uDataJson.uid).emit('changeRole', uDataJson.role);
    });

    socket.on('resetUserList', (data) => {
        console.log('User List Lengh111: ' + uidlist.length);

        uidList.length = 0;
        traineeNum = 0;
        roleNum = 0;
        console.log('User List Lengh222: ' + uidlist.length);
    });

    socket.on('test', () => {
        console.log('TEST TEST');
    });


});



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
