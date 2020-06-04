#include "python.h"
#include <iostream>
#include <numpy/arrayobject.h>

PyObject* pBaseModule = NULL;//声明变量
PyObject* PyArrayIn;
PyObject* PyArrayOut;
//unsigned char* inByte;
//unsigned char* outByte;
unsigned char* inByte;
unsigned char* outByte;

extern "C" _declspec(dllexport) int pyInit2(char* path,  int inLen, int outLen, void ** out)
{

	PyObject* ArgList = NULL;
	PyObject* pModelRunFunc;

	inByte = (unsigned char*)malloc(inLen);
	outByte = (unsigned char*)malloc(outLen);
	out[0] = inByte;
	out[1] = outByte;

	

	Py_SetPythonHome((const wchar_t*)path);
	Py_Initialize();//使用python之前，要调用Py_Initialize();这个函数进行初始化
	import_array();
	PyRun_SimpleString("print('hello')");

	pBaseModule = PyImport_ImportModule("baseDll");//这里是要调用的文件名

	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitInByte");
	npy_intp Dims[1] = { inLen }; //注意这个维度数据！
	PyArrayIn = PyArray_SimpleNewFromData(1, Dims, NPY_UBYTE, inByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayIn);
	PyEval_CallObject(pModelRunFunc, ArgList);//调用函数


	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitOutByte");
	npy_intp Dims2[1] = { outLen/4 }; //注意这个维度数据！
	PyArrayOut = PyArray_SimpleNewFromData(1, Dims2, NPY_FLOAT, outByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayOut);
	PyEval_CallObject(pModelRunFunc, ArgList);//调用函数


	std::cout << "Hello world!" << std::endl;
	std::cout << inByte << std::endl;
	std::cout << " " << std::endl;
	std::cout << outByte << std::endl;

	return 0;
}


extern "C" _declspec(dllexport) int pyInit(char* path, unsigned char* inByte, int inLen, unsigned char* outByte, int outLen)
{
	
	PyObject* ArgList = NULL;
	PyObject* pModelRunFunc;
	

	Py_SetPythonHome((const wchar_t*)path);
	Py_Initialize();//使用python之前，要调用Py_Initialize();这个函数进行初始化
	import_array();
	PyRun_SimpleString("print('hello')");

	pBaseModule = PyImport_ImportModule("baseDll");//这里是要调用的文件名

	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitInByte");
	npy_intp Dims[1] = { inLen }; //注意这个维度数据！
	PyArrayIn = PyArray_SimpleNewFromData(1, Dims, NPY_UBYTE, inByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayIn);
	PyEval_CallObject(pModelRunFunc, ArgList);//调用函数


	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitOutByte");
	npy_intp Dims2[1] = { outLen }; //注意这个维度数据！
	PyArrayOut = PyArray_SimpleNewFromData(1, Dims2, NPY_FLOAT, outByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayOut);
	PyEval_CallObject(pModelRunFunc, ArgList);//调用函数


	return 0;
}

//运行一行python语句
extern "C" _declspec(dllexport) int  PyRunString(char* s)
{
	PyRun_SimpleString(s);
	return 0;
}

////导入一个py文件
//extern "C" _declspec(dllexport) void* PyImportModule(char* s)
//{
//	return PyImport_ImportModule(s);
//}
//
////获取某个modle中的一个函数
//extern "C" _declspec(dllexport) void* PyObject_GetAttrString(void * pModule, char* s)
//{
//
//	return PyObject_GetAttrString(pModule, s);
//}

extern "C" _declspec(dllexport) int  pyDeinit()
{
	Py_Finalize();
	return 0;
}

