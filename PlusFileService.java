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

import org.gocom.components.nui.sample.ntree.PlusFile;

import com.eos.das.entity.ExpressionHelper;
import com.eos.das.entity.IDASCriteria;
import com.eos.das.entity.criteria.CriteriaType;
import com.eos.spring.DASDaoSupport;
import com.eos.system.utility.StringUtil;

import commonj.sdo.DataObject;

/**
 * 实现树形控件(节点)的增删改查
 *
 * @author huangct (mailto:huangct@primeton.com)
 */
public class PlusFileService extends DASDaoSupport implements IPlusFileService {
	
	public void addPlusFile(PlusFile plusFile) {
		getDASTemplate().getPrimaryKey(plusFile);
		getDASTemplate().insertEntity(plusFile);
	}

	public void deletePlusFile(PlusFile[] plusFiles) {
		for (DataObject plusFile : plusFiles) {
			getDASTemplate().deleteEntityCascade(plusFile);
		}

	}

	public void getPlusFile(PlusFile plusFile) {
		getDASTemplate().expandEntity(plusFile);
	}

	public PlusFile[] queryPlusFiles(CriteriaType criteriaType, int begin,
			int length) {
		criteriaType.set_entity(PlusFile.QNAME);
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(
				criteriaType);
		PlusFile[] results = getDASTemplate().queryEntitiesByCriteriaEntity(
				PlusFile.class, dasCriteria, begin, length);
		return results;
	}

	public void updatePlusFile(PlusFile plusFile) {
		getDASTemplate().updateEntity(plusFile);
	}

	public int countFiles(CriteriaType criteria) {
		criteria.set_entity(PlusFile.QNAME);
		IDASCriteria dasCriteria = getDASTemplate().criteriaTypeToDASCriteria(
				criteria);
		return getDASTemplate().count(dasCriteria);
	}

	public void saveTree(PlusFile[] plusFile) {

		for (PlusFile file : plusFile) {
			String state = (String) file.get("_state");
			if (state.equals("added")) {
				addPlusFile(file);

			}

			else if (state.equals("removed")) {

				// deletePlusFile(file);
			}

			else if (state.equals("modified")) // 更新：_state为空，或modified
			{

				updatePlusFile(file);
			}
		}
	}

	public PlusFile[] loadNodes(String id) {
		if (StringUtil.isBlank(id)) {
			id = "-1";
		}

		PlusFile[] results = null;
		IDASCriteria dasCriteria = getDASTemplate().createCriteria(
				PlusFile.QNAME);
		dasCriteria.add(ExpressionHelper.eq("pid", id));
		results = getDASTemplate().queryEntitiesByCriteriaEntity(
				PlusFile.class, dasCriteria);

		for (PlusFile plusFile : results) {
			IDASCriteria dasCriteria2 = getDASTemplate().createCriteria(
					PlusFile.QNAME);
			dasCriteria2.add(ExpressionHelper.eq("pid", plusFile.getId()));
			PlusFile[] children = getDASTemplate()
					.queryEntitiesByCriteriaEntity(PlusFile.class, dasCriteria2);
			if (children.length > 0) {
				plusFile.setBoolean("isLeaf", false);
				plusFile.setBoolean("expanded", false);
			}
		}

		return results;
	}
}
