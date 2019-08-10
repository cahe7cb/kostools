const {AutoLanguageClient} = require('atom-languageclient')
const cp = require('child_process')

class OmniKOSLanguageClient extends AutoLanguageClient {
  getGrammarScopes () { return ['source.kerboscript']; }
  getLanguageName () { return 'KerboScript'; }
  getServerName () {  return 'KOS OmniLanguageServer'; }
  getConnectionType() {  return 'stdio'; }
  startServerProcess () {
    if(atom.config.get('ide-omnikos.devmode')) {
      const sourcepath = atom.config.get('ide-omnikos.sourcepath');
      const server = sourcepath + "/kostools/bin/Debug/kostools.exe";
      if(atom.config.get('ide-omnikos.monoruntime')) {
        return cp.spawn("mono", [server]);
      }
      else {
        return cp.spawn(server);
      }
    }
    const ksppath = atom.config.get("ide-omnikos.ksppath");
    const monopath = [
      ksppath + '/GameData/kOS/Plugins',
      ksppath + '/KSP_Data/Managed'
    ];
    return cp.spawn(ksppath + '/OmniKOS', [], {
      cwd: ksppath,
      env: {
        MONO_PATH: monopath.join(':')
      },
      windowsHide: true
    });
  }
  preInitialization(_connection) {
    super.preInitialization(_connection);
  }
  postInitialization(_connection) {
    super.postInitialization(_connection);
  }
  activate() {
    super.activate()
  }
  shouldStartForEditor(editor) {
    return true;
  }
  constructor() {
    super();
    this.config = {
      ksppath: {
        title: 'Location of your KSP installation (containing kOS and the language server)',
        type: 'string',
        default: 'steam/Kerbal Space Program'
      },
      devmode: {
        title: 'Custom language server',
        description: 'Enable this to use a custom or development version of the language server. A restart is required for this change to take effect.',
        type: 'boolean',
        default: false
      }
    };
    if(atom.config.get('ide-omnikos.devmode')) {
      this.config.sourcepath = {
        title: 'Location of the source project of the language server',
        type: 'string',
        default: 'code/kostools'
      };
      this.config.monoruntime = {
        title: 'Use Mono runtime',
        description: 'Enable to use Mono runtime when running the development server',
        type: 'boolean',
        default: true
      };
    }
  }
  getInitializeParams(path, process) {
    let params = super.getInitializeParams(path, process);
    return params;
  }
}

module.exports = new OmniKOSLanguageClient();
