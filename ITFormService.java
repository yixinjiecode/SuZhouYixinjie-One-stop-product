/*
 * Copyright 2013 Primeton.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.gocom.components.nui.demo;

import com.eos.das.entity.criteria.CriteriaType;

import org.gocom.components.nui.demo.newdataset.TForm;

/**
 * TODO 实现表单控件的增删改查功能
 *
 * @author huangct (mailto:huangct@primeton.com)
 */
public interface ITFormService{

	/**
	 *
	 * @param tForm TForm
	 */
	void addTForm(TForm tForm);

	/**
	 *
	 * @param tForms TForm[]
	 */
	void deleteTForm(TForm[] tForms);

	/**
	 *
	 * @param tForm TForm[]
	 */
	void getTForm(TForm tForm);

	/**
	 *
	 * @param criteria CriteriaType
	 * @param page PageCond
	 * @return TForm[]
	 */
	TForm[] queryTForms(CriteriaType criteriaType,
			 int begin, int length);

	/**
	 *
	 * @param tForm TForm[]
	 */
	void updateTForm(TForm tForm);
	

	/**
	 *
	 * @param tForm TForm[]
	 */
	void saveTForm(TForm[] tform);
	
	void deleteTForm(TForm form);
	
	int countForms(CriteriaType criteria);

}

