#include "python.h"
#include <iostream>
#include <numpy/arrayobject.h>

PyObject* pBaseModule = NULL;//��������
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
	Py_Initialize();//ʹ��python֮ǰ��Ҫ����Py_Initialize();����������г�ʼ��
	import_array();
	PyRun_SimpleString("print('hello')");

	pBaseModule = PyImport_ImportModule("baseDll");//������Ҫ���õ��ļ���

	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitInByte");
	npy_intp Dims[1] = { inLen }; //ע�����ά�����ݣ�
	PyArrayIn = PyArray_SimpleNewFromData(1, Dims, NPY_UBYTE, inByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayIn);
	PyEval_CallObject(pModelRunFunc, ArgList);//���ú���


	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitOutByte");
	npy_intp Dims2[1] = { outLen/4 }; //ע�����ά�����ݣ�
	PyArrayOut = PyArray_SimpleNewFromData(1, Dims2, NPY_FLOAT, outByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayOut);
	PyEval_CallObject(pModelRunFunc, ArgList);//���ú���


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
	Py_Initialize();//ʹ��python֮ǰ��Ҫ����Py_Initialize();����������г�ʼ��
	import_array();
	PyRun_SimpleString("print('hello')");

	pBaseModule = PyImport_ImportModule("baseDll");//������Ҫ���õ��ļ���

	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitInByte");
	npy_intp Dims[1] = { inLen }; //ע�����ά�����ݣ�
	PyArrayIn = PyArray_SimpleNewFromData(1, Dims, NPY_UBYTE, inByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayIn);
	PyEval_CallObject(pModelRunFunc, ArgList);//���ú���


	pModelRunFunc = PyObject_GetAttrString(pBaseModule, "InitOutByte");
	npy_intp Dims2[1] = { outLen }; //ע�����ά�����ݣ�
	PyArrayOut = PyArray_SimpleNewFromData(1, Dims2, NPY_FLOAT, outByte);
	ArgList = PyTuple_New(1);
	PyTuple_SetItem(ArgList, 0, PyArrayOut);
	PyEval_CallObject(pModelRunFunc, ArgList);//���ú���


	return 0;
}

//����һ��python���
extern "C" _declspec(dllexport) int  PyRunString(char* s)
{
	PyRun_SimpleString(s);
	return 0;
}

////����һ��py�ļ�
//extern "C" _declspec(dllexport) void* PyImportModule(char* s)
//{
//	return PyImport_ImportModule(s);
//}
//
////��ȡĳ��modle�е�һ������
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

