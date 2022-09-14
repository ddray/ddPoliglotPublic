import 'package:ddpoliglot_v6_flutter/exeptions/custom_exception.dart';
import 'package:ddpoliglot_v6_flutter/utils/utils.dart';
import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:provider/provider.dart';
import 'package:flutter_facebook_auth/flutter_facebook_auth.dart';
import 'package:path_provider/path_provider.dart' as syspath;
import 'dart:io' as io;

import '../providers/auth/auth_provider.dart';

enum AuthMode { signup, login }

class AuthScreen extends StatelessWidget {
  static const routeName = '/auth';

  const AuthScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final deviceSize = MediaQuery.of(context).size;
    debugPrint('deviceSize.width: ${deviceSize.width}');
    return Scaffold(
      body: Stack(
        children: <Widget>[
          Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  const Color.fromRGBO(16, 16, 35, 1).withOpacity(1),
                  const Color.fromRGBO(72, 72, 95, 1).withOpacity(0.1),
                ],
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                stops: const [0, 1],
              ),
            ),
          ),
          SingleChildScrollView(
            child: SizedBox(
              height: deviceSize.height,
              width: deviceSize.width,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: <Widget>[
                  Flexible(
                    flex: deviceSize.width > 600 ? 2 : 2,
                    child: const AuthCard(),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class AuthCard extends StatefulWidget {
  const AuthCard({
    Key? key,
  }) : super(key: key);

  @override
  AuthCardState createState() => AuthCardState();
}

class AuthCardState extends State<AuthCard> {
  final GlobalKey<FormState> _formKey = GlobalKey();
  bool _passwordVisible = false;
  AuthMode _authMode = AuthMode.login;
  // ignore: prefer_final_fields
  Map<String, String> _authState = {
    'email': '',
    'password': '',
  };
  var _isLoading = false;

  // final _loginController = TextEditingController(
  //     text: (HttpUtils.isRealDevice ?? false) ? '' : 'DimaDry_33@gmail.com');
  // final _passwordController = TextEditingController(
  //     text: (HttpUtils.isRealDevice ?? false) ? '' : 'DimaDry_33');

  final _loginController = TextEditingController(text: '');
  final _passwordController = TextEditingController(text: '');

  final _confirmPasswordController = TextEditingController()..text = '';

  // @override
  // void initState() {
  //   super.initState();
  //   _googleSignIn.onCurrentUserChanged.listen((GoogleSignInAccount? account) {
  //     if ((account?.email ?? '').isNotEmpty) {
  //       context.read<AuthProvider>().loginGoogle(account);
  //     }
  //   });

  //   _googleSignIn.signInSilently();
  // }

  // void _showErrorDialog(String message) {
  //   showDialog(
  //     context: context,
  //     builder: (ctx) => AlertDialog(
  //       title: const Text('Ошибка!'),
  //       content: Text(message),
  //       actions: <Widget>[
  //         TextButton(
  //           child: const Text('Ok'),
  //           onPressed: () {
  //             Navigator.of(ctx).pop();
  //           },
  //         )
  //       ],
  //     ),
  //   );
  // }

  // void _showFinalDialog() {
  //   showDialog(
  //     context: context,
  //     builder: (ctx) => AlertDialog(
  //       title: const Text('Вы успешно зарегистрировались на сайте!'),
  //       content: Column(
  //         children: const [
  //           Text('На Ваш адрес было послано письмо.'),
  //           Text('Перейдите по ссылке в нем для подтверждения Вашего Email.'),
  //         ],
  //       ),
  //       actions: <Widget>[
  //         TextButton(
  //           child: const Text('Okay'),
  //           onPressed: () {
  //             Navigator.of(ctx).pop();
  //           },
  //         )
  //       ],
  //     ),
  //   );
  // }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      // Invalid!
      return;
    }
    _formKey.currentState!.save();
    setState(() {
      _isLoading = true;
    });

    var errorMessage = '';

    try {
      if (_authMode == AuthMode.login) {
        await context.read<AuthProvider>().login(
              _authState['email'] ?? '',
              _authState['password'] ?? '',
            );
      } else {
        await context.read<AuthProvider>().signup(
              _authState['email'] ?? '',
              _authState['password'] ?? '',
            );

        setState(() {
          _authMode = AuthMode.login;
          _isLoading = false;
        });
      }
    } on CustomException catch (error) {
      errorMessage = error.message;
    } catch (error) {
      errorMessage = 'Упс..., произошла ошибка. Попробуйте еще раз';
      rethrow;
    } finally {
      if (errorMessage.isNotEmpty) {
        Utils.showErrorDialog(errorMessage, context);
      }
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _submitAnonim() async {
    setState(() {
      _isLoading = true;
    });
    var errorMessage = '';
    try {
      await context.read<AuthProvider>().loginAnonim();
    } on CustomException catch (error) {
      errorMessage = error.message;
    } catch (error) {
      errorMessage = 'Упс..., произошла ошибка. Попробуйте еще раз';
      rethrow;
    } finally {
      if (errorMessage.isNotEmpty) {
        Utils.showErrorDialog(errorMessage, context);
      }
      setState(() {
        _isLoading = false;
      });
    }
  }

  void _switchAuthMode() {
    if (_authMode == AuthMode.login) {
      setState(() {
        _authMode = AuthMode.signup;
      });
    } else {
      setState(() {
        _authMode = AuthMode.login;
      });
    }
  }

  final GoogleSignIn _googleSignIn = GoogleSignIn();

  @override
  Widget build(BuildContext context) {
    final deviceSize = MediaQuery.of(context).size;
    final auth = context.read<AuthProvider>();

    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(10.0),
      ),
      elevation: 8.0,
      child: Container(
        //margin: EdgeInsets.symmetric(vertical: 30),
        // height: _authMode == AuthMode.signup ? 360 : 360,
        // constraints:
        //     BoxConstraints(minHeight: _authMode == AuthMode.signup ? 420 : 460),
        width: deviceSize.width * 0.90,
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              children: [
                TextFormField(
                  // controller: TextEditingController()..text = defaultUserName,
                  controller: _loginController,
                  decoration: const InputDecoration(labelText: 'E-Mail'),
                  keyboardType: TextInputType.emailAddress,
                  validator: (value) {
                    if ((value ?? '').isEmpty || !(value ?? '').contains('@')) {
                      return 'Invalid email!';
                    }

                    var isValid = RegExp(
                            r"^[a-zA-Z0-9.a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9]+\.[a-zA-Z]+")
                        .hasMatch(value!);
                    if (!isValid) {
                      return 'Invalid email!';
                    }

                    return null;
                  },
                  onSaved: (value) {
                    _authState['email'] = (value ?? '');
                  },
                ),
                TextFormField(
                  decoration: InputDecoration(
                    labelText: 'Password',
                    errorMaxLines: 3,
                    suffixIcon: IconButton(
                        onPressed: () {
                          setState(() {
                            _passwordVisible = !_passwordVisible;
                          });
                        },
                        icon: Icon(_passwordVisible
                            ? Icons.visibility
                            : Icons.visibility_off)),
                  ),
                  obscureText: !_passwordVisible,
                  controller: _passwordController,
                  validator: (value) {
                    if ((value ?? '').isEmpty) {
                      return 'required';
                    }

                    if ((value ?? '').length < 4) {
                      return 'less then 4 chars';
                    }
                    // var isValid = RegExp(
                    //         r"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!@#\$&*~_]).{6,}$")
                    // .hasMatch(value!);
                    // if (!isValid) {
                    //   return 'At least: one upper, one lower, one digit, one Special, 6 chars in lenght';
                    // }

                    return null;
                  },
                  onSaved: (value) {
                    _authState['password'] = (value ?? '');
                  },
                ),
                if (_authMode == AuthMode.signup)
                  TextFormField(
                    controller: _confirmPasswordController,
                    enabled: _authMode == AuthMode.signup,
                    decoration:
                        const InputDecoration(labelText: 'Confirm Password'),
                    obscureText: !_passwordVisible,
                    validator: _authMode == AuthMode.signup
                        ? (value) {
                            if (value != _passwordController.text) {
                              return 'Passwords do not match!';
                            } else {
                              return null;
                            }
                          }
                        : null,
                  ),
                const SizedBox(
                  height: 10,
                ),
                if (_isLoading)
                  const CircularProgressIndicator()
                else
                  Container(
                    padding: const EdgeInsets.only(top: 10),
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: _submit,
                      child: Text(
                        _authMode == AuthMode.login ? 'LOGIN' : 'SIGN UP',
                      ),
                    ),
                  ),
                Container(
                  padding: const EdgeInsets.only(top: 10),
                  width: double.infinity,
                  child: TextButton(
                    onPressed: _switchAuthMode,
                    child: Text(
                      '${_authMode == AuthMode.login ? 'SIGNUP' : 'LOGIN'} INSTEAD',
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.only(top: 10),
                  width: double.infinity,
                  child: auth.state.isNeedAuthAnonim
                      ? TextButton(
                          child: const Text(
                            'Продолжить без аккаунта',
                            textAlign: TextAlign.center,
                          ),
                          onPressed: () => auth.setIsNeedAuthAnonim(false),
                        )
                      : TextButton(
                          onPressed: _submitAnonim,
                          child: const Text(
                            'Вход без аккаунта',
                            textAlign: TextAlign.center,
                          ),
                        ),
                ),
                Container(
                  padding: const EdgeInsets.only(top: 10),
                  width: double.infinity,
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                    children: [
                      FloatingActionButton(
                        backgroundColor: Colors.white,
                        child: SizedBox(
                          height: 30,
                          child: Image.asset('google_PNG19635.png'),
                        ),
                        onPressed: () {
                          _googleSignIn.signIn().then((value) {
                            if ((value?.email ?? '').isNotEmpty) {
                              auth.loginGoogle(value);
                            }
                          });
                        },
                      ),
                      FloatingActionButton(
                        backgroundColor: Colors.white,
                        child: SizedBox(
                          height: 30,
                          child: Image.asset('facebook_logo.png'),
                        ),
                        onPressed: () {
                          FacebookAuth.instance.login().then((value) {
                            if (value.status == LoginStatus.success) {
                              FacebookAuth.instance
                                  .getUserData()
                                  .then((userData) {
                                final email = userData['email'];
                                debugPrint(email);
                                auth.loginFacebook(email);
                              });
                              // you are logged
                            } else {
                              debugPrint(value.status.toString());
                              debugPrint(value.message);
                            }
                          });
                        },
                      ),
                    ],
                  ),
                ),
                if (_isLoading)
                  const CircularProgressIndicator()
                else
                  GestureDetector(
                    child: const Text(
                      'version 1.0.26+26',
                      style: TextStyle(color: Colors.amber),
                    ),
                    onDoubleTap: () async {
                      setState(() {
                        _isLoading = true;
                      });
                      await _deleteCachedFiles();
                      setState(() {
                        _isLoading = false;
                      });
                    },
                  )
              ],
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _deleteCachedFiles() async {
    // delete all files from cashe

    final appDir = await syspath.getApplicationDocumentsDirectory();
    final dir = io.Directory(appDir.path);
    final List<io.FileSystemEntity> entities = await dir.list().toList();
    final Iterable<io.File> files = entities.whereType<io.File>();
    // final List<String> filesToDelete = [];
    for (io.File file in files) {
      final filename = file.path.split(io.Platform.pathSeparator).last;
      final ext = filename.split('.').last;
      if (ext.toLowerCase() == 'mp3') {
        debugPrint('delete: ${file.path}');
        await file.delete();

        // var isDeleted = filesToDelete.where((x) => x == file.path);
        // if (isDeleted.isEmpty) {
        //   debugPrint('delete: ${file.path}');
        //   await file.delete();
        //   filesToDelete.add(file.path);
        // }
      }
    }
    debugPrint('delete: end');
  }
}
