BEGIN_FUNCTION_MAP
	.Func,��ǰ���������ڸŸŵ���(íƮ��),t2545,attr,block,headtype=A;
	BEGIN_DATA_MAP
	t2545InBlock,�⺻�Է�,input;
	begin
		��ǰID,eitem,eitem,char,2;
		���屸��,sgubun,sgubun,char,1;
		�����ڵ�,upcode,upcode,char,3;
		N��,nmin,nmin,int,2;
		��ȸ�Ǽ�,cnt,cnt,int,3;
		���Ϻ�,bgubun,bgubun,char,1;
	end
	t2545OutBlock,�⺻���,output;
	begin
		��ǰID,eitem,eitem,char,2;
		���屸��,sgubun,sgubun,char,1;
		�����������ڵ�,indcode,indcode,char,4;
		�ܱ����������ڵ�,forcode,forcode,char,4;
		������������ڵ�,syscode,syscode,char,4;
		�����������ڵ�,stocode,stocode,char,4;
		�����������ڵ�,invcode,invcode,char,4;
		�����������ڵ�,bancode,bancode,char,4;
		�����������ڵ�,inscode,inscode,char,4;
		�����������ڵ�,fincode,fincode,char,4;
		����������ڵ�,moncode,moncode,char,4;
		��Ÿ�������ڵ�,etccode,etccode,char,4;
		�����������ڵ�,natcode,natcode,char,4;
		����ݵ��������ڵ�,pefcode,pefcode,char,4;
		���������ڵ�,jisucd,jisucd,char,8;
		����������,jisunm,jisunm,char,20;
	end
	t2545OutBlock1,���1,output,occurs;
	begin
		����,date,date,char,8;
		�ð�,time,time,char,6;
		���ڽð�,datetime,datetime,char,14;
		���μ��ż��ŷ���,indmsvol,indmsvol,long,8;
		���μ��ż��ŷ����,indmsamt,indmsamt,double,12.0;
		�ܱ��μ��ż��ŷ���,formsvol,formsvol,long,8;
		�ܱ��μ��ż��ŷ����,formsamt,formsamt,double,12.0;
		�������ż��ŷ���,sysmsvol,sysmsvol,long,8;
		�������ż��ŷ����,sysmsamt,sysmsamt,double,12.0;
		���Ǽ��ż��ŷ���,stomsvol,stomsvol,long,8;
		���Ǽ��ż��ŷ����,stomsamt,stomsamt,double,12.0;
		���ż��ż��ŷ���,invmsvol,invmsvol,long,8;
		���ż��ż��ŷ����,invmsamt,invmsamt,double,12.0;
		������ż��ŷ���,banmsvol,banmsvol,long,8;
		������ż��ŷ����,banmsamt,banmsamt,double,12.0;
		������ż��ŷ���,insmsvol,insmsvol,long,8;
		������ż��ŷ����,insmsamt,insmsamt,double,12.0;
		���ݼ��ż��ŷ���,finmsvol,finmsvol,long,8;
		���ݼ��ż��ŷ����,finmsamt,finmsamt,double,12.0;
		��ݼ��ż��ŷ���,monmsvol,monmsvol,long,8;
		��ݼ��ż��ŷ����,monmsamt,monmsamt,double,12.0;
		��Ÿ���ż��ŷ���,etcmsvol,etcmsvol,long,8;
		��Ÿ���ż��ŷ����,etcmsamt,etcmsamt,double,12.0;
		�������ż��ŷ���,natmsvol,natmsvol,long,8;
		�������ż��ŷ����,natmsamt,natmsamt,double,12.0;
		����ݵ���ż��ŷ���,pefmsvol,pefmsvol,long,8;
		����ݵ���ż��ŷ����,pefmsamt,pefmsamt,double,12.0;
		��������,upclose,upclose,float,6.2;
		����ü��ŷ���,upcvolume,upcvolume,long,8;
		���ش����ŷ���,upvolume,upvolume,double,12.0;
		���ذŷ����,upvalue,upvalue,double,12.0;
	end
	END_DATA_MAP
END_FUNCTION_MAP
