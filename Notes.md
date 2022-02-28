== FejdStartup class is the main menu and entry point for server ==

FejdStartup::Awake()
- entry point for server
- gets command line args and creates the world and networking code

FejdStartup::Start()
- entry point for client
- initialize gui, input, objects, starts main menu
- loads all character files from disk GetAllPlayerProfiles()
- default AppData\LocalLow\IronGate\Valheim\characters 

FejdStartup::OnStartGame()
- client clicks main menu start button
- starts character selection

FejdStartup::OnCharacterStart() when player hits start on character selection
- gets selected player from loaded characters array
- sets PlayerPrefs["profile"] selected player filename
- sets Game.SetProfile selected player filename
- opens world selection / join menu

FejdStartup::OnWorldStart() when player hits start on world selection screen
- passes ZNet the world filename
- transitions to Game 

FejdStartup::OnJoinStart() > FejdStartup::JoinServer() when player joins a community server
- ZNet sets server to null
- ZNet sets serverhost to steamhostid from server list
- transitions to Game
== ZNet class is the networking code ==
ZNet::Awake()
- if server
-- reads adminlist.txt, bannedlist.txt, permittedlist.txt
-- if open (visible to community) open socket and register to matchmaking list
-- WorldGenerator::Initialize(world file name)
-- ZNet::LoadWorld()

- if client
-- ZNet::Connect(steam server id)

ZNet::Connect()
- creates ZNetPeer
- ZNet::OnNewConnection(peer)

ZNet::OnNewConnection()
- sets up rpc connecitons to server
- invokes ZNet::RPC_ServerHandshake()

ZNet::RPC_ServerHandshake()
ZNet::RPC_ClientHandshake()
ZNet::SendPeerInfo()
- server password dialog
- send password and character name

ZNet::RPC_PeerInfo() finish server/client connection here
- if server 
-- checks peer is valid (version, password, ban, whitelist, #players, already connected)

- if client
-- setup local world
-- WorldGenerator::Initialize()

-- finished connection!
ZNet.m_connectionStatus = ZNet.ConnectionStatus.Connected;

- ZDOMan::AddPeer()
-- Add peer to object manager so we keep all world objects in sync
- ZRoutedRpc::AddPeer()
-- Add to rpc peers so we can call rpcs on it


-----------------------

Use ZDO object (e.g. one is present inside Character) to store your custom data, which will be properly synchronized, saved and loaded automatically.
I can make an educated guess that it shouldn't break any compatibility if you make sure that your custom data's key is unique.

ZDO behavior for Player differs from Character, since Player behavior is unique, it is not stored inside the map file as other ZDO's do, therefore any ZDO's created on Player will be lost on respawn, logout, etc.

Haven't tested it much yet, please tell me if I'm wrong about something. 

--------------------------